using System.Timers;
using System.Threading;
using hinos_api.DTOs;
using hinos_api.Models;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace hinos_api.Services;

public class WarCrySyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WarCrySyncBackgroundService>? _logger;
    private readonly int _intervalMinutes;
    private readonly bool _enabled;
    private Timer? _timer;
    private readonly SemaphoreSlim _syncLock = new(1, 1);
    private bool _isRunning;
    private DateTime? _lastSyncStart;
    private DateTime? _lastSyncEnd;
    private int _totalWarCries;
    private string? _lastError;
    private readonly List<string> _recentFiles = new();

    public WarCrySyncBackgroundService(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<WarCrySyncBackgroundService>? logger = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        
        _intervalMinutes = configuration.GetValue("OneDriveSync:IntervalMinutes", 60);
        _enabled = configuration.GetValue("OneDriveSync:Enabled", true);
    }

    public bool IsRunning => _isRunning;
    public DateTime? LastSyncStart => _lastSyncStart;
    public DateTime? LastSyncEnd => _lastSyncEnd;
    public int TotalWarCries => _totalWarCries;
    public string? LastError => _lastError;
    public List<string> RecentFiles => _recentFiles;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger?.LogInformation("Serviço de sincronização de Gritos de Guerra está desabilitado");
            return;
        }

        _logger?.LogInformation("Serviço de sincronização de Gritos de Guerra iniciado. Intervalo: {Interval} minutos", _intervalMinutes);

        using var scope = _scopeFactory.CreateScope();
        var warCryService = scope.ServiceProvider.GetRequiredService<WarCryService>();
        _totalWarCries = await warCryService.GetTotalCountAsync();

        _timer = new Timer(_intervalMinutes * 60 * 1000);
        _timer.Elapsed += async (sender, args) => await SyncAsync(incremental: true);
        _timer.AutoReset = true;
        _timer.Start();

        await SyncAsync(incremental: true);
    }

    public async Task<WarCrySyncStatusDto> SyncAsync(bool incremental = true)
    {
        if (!await _syncLock.WaitAsync(0))
        {
            _logger?.LogWarning("Sincronização já está em andamento");
            return new WarCrySyncStatusDto
            {
                IsRunning = true,
                LastSyncStart = _lastSyncStart,
                LastSyncEnd = _lastSyncEnd,
                TotalWarCries = _totalWarCries,
                LastError = "Sincronização já está em andamento"
            };
        }

        _isRunning = true;
        _lastSyncStart = DateTime.UtcNow;
        _lastError = null;
        _recentFiles.Clear();

        var syncLog = new WarCrySyncLogDto
        {
            SyncStartTime = _lastSyncStart.Value,
            SyncType = incremental ? "incremental" : "full"
        };

        try
        {
            _logger?.LogInformation("Iniciando sincronização {Type} de Gritos de Guerra...", incremental ? "incremental" : "completa");

            using var scope = _scopeFactory.CreateScope();
            var oneDriveSync = scope.ServiceProvider.GetRequiredService<OneDriveSyncService>();
            var warCryService = scope.ServiceProvider.GetRequiredService<WarCryService>();
            var pdfExtractor = scope.ServiceProvider.GetRequiredService<PdfExtractorService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.HymnsDbContext>();

            var fileListResult = await oneDriveSync.ListWarCryFilesAsync();
            
            if (!fileListResult.Success)
            {
                _lastError = fileListResult.ErrorMessage;
                syncLog.ErrorMessage = _lastError;
                syncLog.Success = false;
                await SaveSyncLogAsync(scope, syncLog);
                return GetStatus();
            }

            syncLog.TotalFilesFound = fileListResult.TotalFilesFound;
            var onedriveFiles = fileListResult.Files;
            var currentFileNames = onedriveFiles.Select(f => f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingWarCries = await warCryService.GetAllEntitiesAsync();
            var existingByFileName = existingWarCries.ToDictionary(w => w.FileName, StringComparer.OrdinalIgnoreCase);

            foreach (var onedriveFile in onedriveFiles)
            {
                try
                {
                    var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(onedriveFile.Name);
                    
                    if (existingByFileName.TryGetValue(onedriveFile.Name, out var existingItem))
                    {
                        var existingWarCry = await dbContext.WarCries.FindAsync(existingItem.Id);
                        
                        if (existingWarCry != null)
                        {
                            if (incremental && existingWarCry.FileSize == onedriveFile.FileSize && existingWarCry.SyncStatus == "active")
                            {
                                syncLog.SkippedCount++;
                                _logger?.LogDebug("Arquivo não alterado (mesmo tamanho): {FileName}", onedriveFile.Name);
                                continue;
                            }

                            var extracted = await pdfExtractor.ExtractTextFromUrlAsync(onedriveFile.DownloadUrl, onedriveFile.Name);
                            
                            if (extracted.Success)
                            {
                                if (existingWarCry.FileHash != extracted.FileHash)
                                {
                                    existingWarCry.Content = extracted.Content;
                                    existingWarCry.FileHash = extracted.FileHash;
                                    existingWarCry.FileSize = extracted.FileSize;
                                    existingWarCry.FileModifiedAt = onedriveFile.LastModified;
                                    existingWarCry.UpdatedAt = DateTime.UtcNow;
                                    existingWarCry.SyncedAt = DateTime.UtcNow;
                                    existingWarCry.SyncStatus = "active";
                                    
                                    syncLog.UpdatedCount++;
                                    _logger?.LogInformation("Arquivo atualizado: {FileName}", onedriveFile.Name);
                                }
                                else
                                {
                                    syncLog.SkippedCount++;
                                    _logger?.LogDebug("Arquivo com mesmo hash, skipped: {FileName}", onedriveFile.Name);
                                }
                            }
                            else
                            {
                                syncLog.FailedCount++;
                                syncLog.FailedFileNames.Add(onedriveFile.Name);
                                _logger?.LogWarning("Falha ao extrair PDF: {FileName} - {Error}", onedriveFile.Name, extracted.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        var extracted = await pdfExtractor.ExtractTextFromUrlAsync(onedriveFile.DownloadUrl, onedriveFile.Name);
                        
                        if (extracted.Success)
                        {
                            var newWarCry = new WarCry
                            {
                                Title = title,
                                FileName = onedriveFile.Name,
                                Content = extracted.Content,
                                MessageNumber = messageNumber ?? 0,
                                Theme = theme,
                                SourcePath = onedriveFile.Path,
                                FileHash = extracted.FileHash,
                                FileSize = extracted.FileSize,
                                FileModifiedAt = onedriveFile.LastModified,
                                SyncedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                SyncStatus = "active"
                            };

                            dbContext.WarCries.Add(newWarCry);
                            syncLog.NewCount++;
                            syncLog.ProcessedFileNames.Add(onedriveFile.Name);
                            _logger?.LogInformation("Novo arquivo inserido: {FileName}", onedriveFile.Name);
                        }
                        else
                        {
                            syncLog.FailedCount++;
                            syncLog.FailedFileNames.Add(onedriveFile.Name);
                            _logger?.LogWarning("Falha ao extrair novo PDF: {FileName} - {Error}", onedriveFile.Name, extracted.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    syncLog.FailedCount++;
                    syncLog.FailedFileNames.Add(onedriveFile.Name);
                    _logger?.LogError(ex, "Erro ao processar arquivo: {FileName}", onedriveFile.Name);
                }
            }

            await dbContext.SaveChangesAsync();

            if (!incremental)
            {
                var allWarCries = await dbContext.WarCries.ToListAsync();
                foreach (var warCry in allWarCries)
                {
                    if (!currentFileNames.Contains(warCry.FileName))
                    {
                        warCry.SyncStatus = "orphan";
                        syncLog.OrphanCount++;
                        _logger?.LogInformation("Arquivo marcado como órfão: {FileName}", warCry.FileName);
                    }
                }
                await dbContext.SaveChangesAsync();
            }

            syncLog.Success = true;
            syncLog.SyncEndTime = DateTime.UtcNow;
            await SaveSyncLogAsync(scope, syncLog);

            _totalWarCries = await warCryService.GetTotalCountAsync();
            _lastSyncEnd = DateTime.UtcNow;
            _recentFiles.AddRange(syncLog.ProcessedFileNames.Take(10));
            
            _logger?.LogInformation(
                "Sincronização concluída. Total: {Total}, Novos: {New}, Atualizados: {Updated}, Ignorados: {Skipped}, Falhas: {Failed}, Órfãos: {Orphan}", 
                _totalWarCries, syncLog.NewCount, syncLog.UpdatedCount, syncLog.SkippedCount, syncLog.FailedCount, syncLog.OrphanCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro durante sincronização");
            _lastError = ex.Message;
            syncLog.ErrorMessage = ex.Message;
            syncLog.Success = false;
            syncLog.SyncEndTime = DateTime.UtcNow;
            
            using var scope = _scopeFactory.CreateScope();
            await SaveSyncLogAsync(scope, syncLog);
            
            _lastSyncEnd = DateTime.UtcNow;
        }
        finally
        {
            _isRunning = false;
            _syncLock.Release();
        }

        return GetStatus();
    }

    private async Task SaveSyncLogAsync(IServiceScope scope, WarCrySyncLogDto log)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Data.HymnsDbContext>();
            
            var logEntry = new SyncLogEntry
            {
                SyncStartTime = log.SyncStartTime,
                SyncEndTime = log.SyncEndTime,
                SyncType = log.SyncType,
                TotalFilesFound = log.TotalFilesFound,
                NewCount = log.NewCount,
                UpdatedCount = log.UpdatedCount,
                SkippedCount = log.SkippedCount,
                FailedCount = log.FailedCount,
                OrphanCount = log.OrphanCount,
                Success = log.Success,
                ErrorMessage = log.ErrorMessage,
                ProcessedFileNames = string.Join(";", log.ProcessedFileNames),
                FailedFileNames = string.Join(";", log.FailedFileNames)
            };

            dbContext.SyncLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Falha ao salvar log de sincronização");
        }
    }

    public WarCrySyncStatusDto GetStatus()
    {
        return new WarCrySyncStatusDto
        {
            IsRunning = _isRunning,
            LastSyncStart = _lastSyncStart,
            LastSyncEnd = _lastSyncEnd,
            TotalWarCries = _totalWarCries,
            NewWarCries = 0,
            UpdatedWarCries = 0,
            FailedCount = 0,
            LastError = _lastError,
            RecentFiles = _recentFiles.Take(10).ToList()
        };
    }

    public override void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _syncLock.Dispose();
        base.Dispose();
    }
}

public class SyncLogEntry
{
    public int Id { get; set; }
    public DateTime SyncStartTime { get; set; }
    public DateTime? SyncEndTime { get; set; }
    public string SyncType { get; set; } = "incremental";
    public int TotalFilesFound { get; set; }
    public int NewCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int FailedCount { get; set; }
    public int OrphanCount { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ProcessedFileNames { get; set; }
    public string? FailedFileNames { get; set; }
}
