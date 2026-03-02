using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.DTOs;
using hinos_api.Models;

namespace hinos_api.Services;

public class WarCryService
{
    private readonly HymnsDbContext _dbContext;
    private readonly ILogger<WarCryService>? _logger;

    public WarCryService(HymnsDbContext dbContext, ILogger<WarCryService>? logger = null)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<WarCryListItemDto>> GetAllAsync(string? search = null)
    {
        var query = _dbContext.WarCries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(w =>
                w.Title.ToLower().Contains(searchLower) ||
                w.FileName.ToLower().Contains(searchLower) ||
                w.Content.ToLower().Contains(searchLower) ||
                (w.Theme != null && w.Theme.ToLower().Contains(searchLower))
            );
        }

        var warCries = await query
            .OrderBy(w => w.MessageNumber)
            .ThenBy(w => w.Title)
            .ToListAsync();

        return warCries.Select(MapToListItem).ToList();
    }

    public async Task<List<WarCry>> GetAllEntitiesAsync()
    {
        return await _dbContext.WarCries
            .OrderBy(w => w.MessageNumber)
            .ThenBy(w => w.Title)
            .ToListAsync();
    }

    public async Task<WarCryResponseDto?> GetByIdAsync(int id)
    {
        var warCry = await _dbContext.WarCries.FindAsync(id);
        return warCry != null ? MapToDto(warCry) : null;
    }

    public async Task<List<WarCryListItemDto>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return await GetAllAsync();
        }

        return await GetAllAsync(term);
    }

    public async Task<WarCrySyncResultDto> ProcessSyncResultAsync(OneDriveSyncResult syncResult)
    {
        var result = new WarCrySyncResultDto
        {
            Success = syncResult.Success,
            SyncStartTime = syncResult.StartTime,
            SyncEndTime = syncResult.EndTime ?? DateTime.UtcNow,
            Message = syncResult.Success ? "Sincronização concluída" : syncResult.ErrorMessage ?? "Erro desconhecido"
        };

        foreach (var processed in syncResult.ProcessedFiles)
        {
            try
            {
                var (messageNumber, theme, title) = OneDriveSyncService.ParseWarCryFileName(processed.FileName);
                
                var existing = await _dbContext.WarCries
                    .FirstOrDefaultAsync(w => w.FileHash == processed.FileHash);

                if (existing != null)
                {
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.SyncedAt = DateTime.UtcNow;
                    result.UpdatedCount++;
                }
                else
                {
                    var newWarCry = new WarCry
                    {
                        Title = title,
                        FileName = processed.FileName,
                        Content = processed.FileHash ?? "",
                        MessageNumber = messageNumber ?? 0,
                        Theme = theme,
                        SourcePath = "",
                        FileHash = processed.FileHash ?? "",
                        FileSize = processed.FileSize,
                        SyncedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    _dbContext.WarCries.Add(newWarCry);
                    result.NewCount++;
                }
                
                result.TotalProcessed++;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao processar arquivo: {FileName}", processed.FileName);
                result.FailedCount++;
            }
        }

        foreach (var failed in syncResult.FailedFiles)
        {
            result.FailedCount++;
            result.TotalProcessed++;
        }

        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbContext.WarCries.CountAsync();
    }

    public async Task<bool> ExistsByHashAsync(string fileHash)
    {
        return await _dbContext.WarCries.AnyAsync(w => w.FileHash == fileHash);
    }

    public async Task UpdateContentAsync(int id, string content, DateTime? fileModifiedAt = null)
    {
        var warCry = await _dbContext.WarCries.FindAsync(id);
        if (warCry != null)
        {
            warCry.Content = content;
            warCry.FileModifiedAt = fileModifiedAt;
            warCry.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var warCry = await _dbContext.WarCries.FindAsync(id);
        if (warCry != null)
        {
            _dbContext.WarCries.Remove(warCry);
            await _dbContext.SaveChangesAsync();
        }
    }

    private static WarCryResponseDto MapToDto(WarCry warCry)
    {
        return new WarCryResponseDto
        {
            Id = warCry.Id,
            Title = warCry.Title,
            FileName = warCry.FileName,
            Content = warCry.Content,
            MessageNumber = warCry.MessageNumber,
            Theme = warCry.Theme,
            SourcePath = warCry.SourcePath,
            FileSize = warCry.FileSize,
            FileModifiedAt = warCry.FileModifiedAt,
            SyncedAt = warCry.SyncedAt,
            CreatedAt = warCry.CreatedAt,
            UpdatedAt = warCry.UpdatedAt
        };
    }

    private static WarCryListItemDto MapToListItem(WarCry warCry)
    {
        var preview = warCry.Content;
        if (preview.Length > 150)
        {
            preview = preview.Substring(0, 150) + "...";
        }
        preview = preview.Replace("\n", " ").Replace("\r", "");

        return new WarCryListItemDto
        {
            Id = warCry.Id,
            Title = warCry.Title,
            MessageNumber = warCry.MessageNumber,
            Theme = warCry.Theme,
            ContentPreview = preview,
            SyncedAt = warCry.SyncedAt
        };
    }
}
