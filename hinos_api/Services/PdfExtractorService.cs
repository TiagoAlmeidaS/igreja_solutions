using System.Security.Cryptography;
using UglyToad.PdfPig;

namespace hinos_api.Services;

public class PdfExtractorService
{
    private readonly ILogger<PdfExtractorService>? _logger;

    public PdfExtractorService(ILogger<PdfExtractorService>? logger = null)
    {
        _logger = logger;
    }

    public async Task<PdfExtractionResult> ExtractTextAsync(Stream pdfStream, string fileName)
    {
        var result = new PdfExtractionResult
        {
            FileName = fileName,
            Success = false
        };

        try
        {
            using var memoryStream = new MemoryStream();
            await pdfStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var bytes = memoryStream.ToArray();
            result.FileSize = bytes.Length;
            result.FileHash = ComputeSha256(bytes);

            using var document = PdfDocument.Open(memoryStream);
            var textBuilder = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
            {
                var pageText = page.Text;
                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    textBuilder.AppendLine(pageText);
                    textBuilder.AppendLine();
                }
            }

            result.Content = textBuilder.ToString().Trim();
            result.PageCount = document.NumberOfPages;
            result.Success = true;

            _logger?.LogInformation("PDF extraído com sucesso: {FileName}, {Pages} páginas, {Size} bytes",
                fileName, result.PageCount, result.FileSize);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao extrair texto do PDF: {FileName}", fileName);
            result.ErrorMessage = ex.Message;
            result.Success = false;
        }

        return result;
    }

    public async Task<PdfExtractionResult> ExtractTextFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new PdfExtractionResult
            {
                FileName = Path.GetFileName(filePath),
                Success = false,
                ErrorMessage = "Arquivo não encontrado"
            };
        }

        await using var stream = File.OpenRead(filePath);
        return await ExtractTextAsync(stream, Path.GetFileName(filePath));
    }

    public async Task<PdfExtractionResult> ExtractTextFromUrlAsync(string url, string fileName)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await ExtractTextAsync(stream, fileName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao baixar PDF da URL: {Url}", url);
            return new PdfExtractionResult
            {
                FileName = fileName,
                Success = false,
                ErrorMessage = $"Erro ao baixar: {ex.Message}"
            };
        }
    }

    private static string ComputeSha256(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

public class PdfExtractionResult
{
    public string FileName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int PageCount { get; set; }
    public string? ErrorMessage { get; set; }
}
