using hinos_api.DTOs;

namespace hinos_api.Services;

public class HymnFormatService
{
    /// <summary>
    /// Gera o texto formatado para Holyrics, OpenLP e outros softwares de projeção
    /// </summary>
    /// <param name="hymn">O hino a ser formatado</param>
    /// <returns>Texto formatado no padrão Holyrics</returns>
    public string GenerateHolyricsText(HymnResponseDto hymn)
    {
        var text = $"#{hymn.Number} - {hymn.Title}\n";
        text += $"{hymn.HymnBook}\n\n";

        for (int i = 0; i < hymn.Verses.Count; i++)
        {
            var verse = hymn.Verses[i];
            text += $"[{verse.Type}]\n";
            
            foreach (var line in verse.Lines)
            {
                text += $"{line}\n";
            }
            
            if (i < hymn.Verses.Count - 1)
            {
                text += "\n";
            }
        }

        if (hymn.Key != null || hymn.Bpm != null)
        {
            text += "\n---\n";
            if (hymn.Key != null) text += $"Tom: {hymn.Key}";
            if (hymn.Key != null && hymn.Bpm != null) text += " | ";
            if (hymn.Bpm != null) text += $"BPM: {hymn.Bpm}";
        }

        return text;
    }

    /// <summary>
    /// Gera o texto sem marcadores (para WhatsApp/uso geral)
    /// </summary>
    /// <param name="hymn">O hino a ser formatado</param>
    /// <returns>Texto plano sem marcadores de tipo</returns>
    public string GeneratePlainText(HymnResponseDto hymn)
    {
        var text = $"{hymn.Title}\n\n";

        for (int i = 0; i < hymn.Verses.Count; i++)
        {
            var verse = hymn.Verses[i];
            
            foreach (var line in verse.Lines)
            {
                text += $"{line}\n";
            }
            
            if (i < hymn.Verses.Count - 1)
            {
                text += "\n";
            }
        }

        return text;
    }

    /// <summary>
    /// Gera um nome de arquivo seguro a partir do número e título do hino
    /// </summary>
    /// <param name="hymn">O hino</param>
    /// <param name="format">Formato do arquivo (plain ou holyrics)</param>
    /// <returns>Nome do arquivo formatado</returns>
    public string GenerateFileName(HymnResponseDto hymn, string format = "holyrics")
    {
        var titleSlug = hymn.Title
            .ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace(":", "-")
            .Replace("*", "")
            .Replace("?", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("|", "-");
        
        // Remove caracteres especiais múltiplos
        while (titleSlug.Contains("--"))
        {
            titleSlug = titleSlug.Replace("--", "-");
        }
        
        titleSlug = titleSlug.Trim('-');
        
        return $"hino-{hymn.Number}-{titleSlug}.txt";
    }
}
