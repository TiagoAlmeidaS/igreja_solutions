using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using hinos_api.DTOs;

namespace hinos_api.Services;

public class OneDriveSyncService
{
    private readonly HttpClient _httpClient;
    private readonly PdfExtractorService _pdfExtractor;
    private readonly ILogger<OneDriveSyncService>? _logger;
    private readonly string _shareUrl;
    private readonly string _accessToken;
    private readonly bool _usePublicAccess;

    private static readonly Regex[] WarCryPatterns = new[]
    {
        new Regex(@"grito\s*de\s*guerra", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"grito\s+de\s+guerra", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"gritodeguerra", RegexOptions.IgnoreCase | RegexOptions.Compiled),
    };

    public OneDriveSyncService(
        IConfiguration configuration,
        PdfExtractorService pdfExtractor,
        ILogger<OneDriveSyncService>? logger = null)
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(10);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Hinos-API/1.0");
        
        _pdfExtractor = pdfExtractor;
        _logger = logger;
        
        _shareUrl = configuration["OneDriveSync:ShareUrl"] ?? string.Empty;
        _accessToken = configuration["OneDriveSync:AccessToken"] ?? string.Empty;
        _usePublicAccess = string.IsNullOrEmpty(_accessToken);
        
        if (!_usePublicAccess)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }

    public async Task<OneDriveFileListResult> ListWarCryFilesAsync()
    {
        var result = new OneDriveFileListResult
        {
            StartTime = DateTime.UtcNow,
            Success = false
        };

        try
        {
            _logger?.LogInformation("Listando arquivos de Grito de Guerra no OneDrive...");

            if (string.IsNullOrEmpty(_shareUrl))
            {
                result.ErrorMessage = "URL do OneDrive não configurada";
                return result;
            }

            var folderId = ExtractFolderId(_shareUrl);
            if (string.IsNullOrEmpty(folderId))
            {
                result.ErrorMessage = "Não foi possível extrair o ID da pasta do OneDrive";
                return result;
            }

            var pdfFiles = await FindWarCryPdfsAsync(folderId);
            result.Files = pdfFiles;
            result.TotalFilesFound = pdfFiles.Count;
            result.Success = true;

            _logger?.LogInformation("Encontrados {Count} arquivos PDF de Grito de Guerra", pdfFiles.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao listar arquivos do OneDrive");
            result.ErrorMessage = ex.Message;
        }

        result.EndTime = DateTime.UtcNow;
        return result;
    }

    public async Task<PdfExtractionResult> DownloadAndExtractAsync(string downloadUrl, string fileName)
    {
        try
        {
            return await _pdfExtractor.ExtractTextFromUrlAsync(downloadUrl, fileName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao baixar PDF: {FileName}", fileName);
            return new PdfExtractionResult
            {
                FileName = fileName,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<OneDriveSyncResult> SyncWarCriesAsync()
    {
        var result = new OneDriveSyncResult
        {
            StartTime = DateTime.UtcNow,
            Success = false
        };

        try
        {
            _logger?.LogInformation("Iniciando sincronização de Gritos de Guerra...");

            if (string.IsNullOrEmpty(_shareUrl))
            {
                result.ErrorMessage = "URL do OneDrive não configurada";
                return result;
            }

            var folderId = ExtractFolderId(_shareUrl);
            if (string.IsNullOrEmpty(folderId))
            {
                result.ErrorMessage = "Não foi possível extrair o ID da pasta do OneDrive";
                return result;
            }

            _logger?.LogInformation("ID da pasta OneDrive: {FolderId}", folderId);

            var pdfFiles = await FindWarCryPdfsAsync(folderId);
            result.TotalFilesFound = pdfFiles.Count;

            _logger?.LogInformation("Encontrados {Count} arquivos PDF de Grito de Guerra", pdfFiles.Count);

            foreach (var pdfFile in pdfFiles)
            {
                try
                {
                    var extracted = await _pdfExtractor.ExtractTextFromUrlAsync(
                        pdfFile.DownloadUrl, 
                        pdfFile.Name);

                    if (extracted.Success)
                    {
                        result.ProcessedFiles.Add(new ProcessedFile
                        {
                            FileName = pdfFile.Name,
                            FileHash = extracted.FileHash,
                            FileSize = extracted.FileSize,
                            DownloadUrl = pdfFile.DownloadUrl,
                            Path = pdfFile.Path,
                            Success = true
                        });
                    }
                    else
                    {
                        result.FailedFiles.Add(new ProcessedFile
                        {
                            FileName = pdfFile.Name,
                            Success = false,
                            ErrorMessage = extracted.ErrorMessage
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro ao processar arquivo: {FileName}", pdfFile.Name);
                    result.FailedFiles.Add(new ProcessedFile
                    {
                        FileName = pdfFile.Name,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }

            result.Success = true;
            result.EndTime = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro durante sincronização do OneDrive");
            result.ErrorMessage = ex.Message;
            result.EndTime = DateTime.UtcNow;
        }

        return result;
    }

    private async Task<List<OneDriveFile>> FindWarCryPdfsAsync(string folderId)
    {
        var pdfFiles = new List<OneDriveFile>();

        await FindWarCryPdfsRecursiveAsync(folderId, pdfFiles);

        return pdfFiles;
    }

    private async Task FindWarCryPdfsRecursiveAsync(string folderId, List<OneDriveFile> results)
    {
        try
        {
            string itemsUrl;
            
            if (_usePublicAccess)
            {
                itemsUrl = $"https://api.onedrive.com/v1.0/shares/{folderId}/root/children";
            }
            else
            {
                itemsUrl = $"https://graph.microsoft.com/v1.0/me/drive/items/{folderId}/children";
            }
            
            while (!string.IsNullOrEmpty(itemsUrl))
            {
                var response = await _httpClient.GetAsync(itemsUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Erro ao listar itens: {StatusCode}", response.StatusCode);
                    break;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("value", out var items))
                {
                    foreach (var item in items.EnumerateArray())
                    {
                        var name = item.GetProperty("name").GetString() ?? "";
                        var isFolder = item.TryGetProperty("folder", out _);
                        
                        if (isFolder)
                        {
                            var childFolderId = item.GetProperty("id").GetString();
                            if (!string.IsNullOrEmpty(childFolderId))
                            {
                                await FindWarCryPdfsRecursiveAsync(childFolderId, results);
                            }
                        }
                        else if (name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            if (IsWarCryFile(name))
                            {
                                string? downloadUrl = null;
                                string itemPath = "";
                                long fileSize = 0;
                                DateTime? lastModified = null;
                                
                                if (_usePublicAccess)
                                {
                                    if (item.TryGetProperty("@microsoft.graph.downloadUrl", out var dlUrl))
                                    {
                                        downloadUrl = dlUrl.GetString();
                                    }
                                    if (item.TryGetProperty("parentReference", out var parentRef) && 
                                        parentRef.TryGetProperty("path", out var path))
                                    {
                                        itemPath = path.GetString() ?? "";
                                    }
                                    if (item.TryGetProperty("size", out var size))
                                    {
                                        fileSize = size.GetInt64();
                                    }
                                    if (item.TryGetProperty("lastModifiedDateTime", out var lastMod))
                                    {
                                        lastModified = lastMod.GetDateTime();
                                    }
                                }
                                else
                                {
                                    if (item.TryGetProperty("@microsoft.graph.downloadUrl", out var dlUrl))
                                    {
                                        downloadUrl = dlUrl.GetString();
                                    }
                                    if (item.TryGetProperty("parentReference", out var parentRef) && 
                                        parentRef.TryGetProperty("path", out var path))
                                    {
                                        itemPath = path.GetString() ?? "";
                                    }
                                    if (item.TryGetProperty("size", out var size))
                                    {
                                        fileSize = size.GetInt64();
                                    }
                                    if (item.TryGetProperty("lastModifiedDateTime", out var lastMod))
                                    {
                                        lastModified = lastMod.GetDateTime();
                                    }
                                }

                                if (!string.IsNullOrEmpty(downloadUrl))
                                {
                                    results.Add(new OneDriveFile
                                    {
                                        Name = name,
                                        DownloadUrl = downloadUrl,
                                        Path = itemPath,
                                        FileSize = fileSize,
                                        LastModified = lastModified
                                    });
                                }
                            }
                        }
                    }
                }

                if (root.TryGetProperty("@odata.nextLink", out var nextLink))
                {
                    itemsUrl = nextLink.GetString() ?? "";
                }
                else
                {
                    itemsUrl = "";
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar PDFs na pasta: {FolderId}", folderId);
        }
    }

    private bool IsWarCryFile(string fileName)
    {
        return WarCryPatterns.Any(pattern => pattern.IsMatch(fileName));
    }

    private string? ExtractFolderId(string shareUrl)
    {
        try
        {
            string shareId;
            
            if (shareUrl.Contains("1drv.ms"))
            {
                var match = Regex.Match(shareUrl, @"1drv\.ms/[^/]+/([^?&]+)");
                if (match.Success)
                {
                    shareId = match.Groups[1].Value;
                }
                else
                {
                    return null;
                }
            }
            else if (shareUrl.Contains("onedrive.live.com"))
            {
                var idMatch = Regex.Match(shareUrl, @"[?&]id=([^&]+)");
                if (idMatch.Success)
                {
                    shareId = Uri.UnescapeDataString(idMatch.Groups[1].Value);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            return "u!" + shareId;
        }
        catch
        {
            return null;
        }
    }

    public static (int? messageNumber, string? theme, string title) ParseWarCryFileName(string fileName)
    {
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        
        int? messageNumber = null;
        string? theme = null;
        string title = baseName;

        var msgMatch = Regex.Match(baseName, @"Msg\s*(\d+)", RegexOptions.IgnoreCase);
        if (msgMatch.Success)
        {
            if (int.TryParse(msgMatch.Groups[1].Value, out var num))
            {
                messageNumber = num;
            }
        }

        var parts = baseName.Split('_');
        
        var themeKeywords = new[] { "Jacó", "Josué", "Moisés", "Davi", "Pedro", "Paulo", "Jesus" };
        foreach (var part in parts)
        {
            foreach (var keyword in themeKeywords)
            {
                if (part.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    theme = keyword;
                    break;
                }
            }
            if (theme != null) break;
        }

        var titlePatterns = new[]
        {
            @"Grito\s*de\s*Guerra.*?_\d+_.*?_(.+)",
            @"Grito\s+de\s+Guerra.*?\d+.*?(.+)",
            @"Grito\s*de\s*Guerra.*?-(.+)",
        };

        foreach (var pattern in titlePatterns)
        {
            var titleMatch = Regex.Match(baseName, pattern, RegexOptions.IgnoreCase);
            if (titleMatch.Success)
            {
                title = titleMatch.Groups[1].Value.Trim();
                break;
            }
        }

        title = Regex.Replace(title, @"^(?:GRUPO\s*)?\d+\s*", "", RegexOptions.IgnoreCase);
        title = title.Trim('_', ' ', '-');

        if (string.IsNullOrWhiteSpace(title))
        {
            title = baseName;
        }

        return (messageNumber, theme, title);
    }
}

public class OneDriveFileListResult
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalFilesFound { get; set; }
    public List<OneDriveFile> Files { get; set; } = new();
}

public class OneDriveSyncResult
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalFilesFound { get; set; }
    public List<ProcessedFile> ProcessedFiles { get; set; } = new();
    public List<ProcessedFile> FailedFiles { get; set; } = new();
}

public class ProcessedFile
{
    public string FileName { get; set; } = string.Empty;
    public string? FileHash { get; set; }
    public long FileSize { get; set; }
    public string? DownloadUrl { get; set; }
    public string? Path { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OneDriveFile
{
    public string Name { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime? LastModified { get; set; }
}
