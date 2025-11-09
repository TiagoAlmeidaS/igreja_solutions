using Microsoft.Data.Sqlite;
using hinos_api.DTOs;
using System.Text.RegularExpressions;
using System.Text;

namespace hinos_api.Services;

public class HinarioSqliteService
{
    private readonly string _dbPath;
    private readonly ILogger<HinarioSqliteService>? _logger;

    /// <summary>
    /// Normaliza um número de hino removendo hífens, espaços e outros caracteres especiais
    /// Exemplo: "S-38" ou "S 38" vira "S38"
    /// </summary>
    private static string NormalizeHymnNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return string.Empty;

        // Remove hífens, espaços, pontos e outros separadores comuns
        var normalized = Regex.Replace(number, @"[\s\-\._]+", "", RegexOptions.Compiled);
        return normalized.ToUpperInvariant();
    }
    
    /// <summary>
    /// Normaliza um número para comparação, removendo todos os caracteres não alfanuméricos
    /// </summary>
    private static string NormalizeForComparison(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return string.Empty;
            
        // Remove tudo exceto letras e números
        return Regex.Replace(number, @"[^a-zA-Z0-9]", "", RegexOptions.Compiled).ToUpperInvariant();
    }

    /// <summary>
    /// Cria múltiplos padrões de busca para um termo, incluindo versões normalizadas
    /// Exemplo: "S-38" gera ["S-38", "S38", "s-38", "s38"]
    /// </summary>
    private static List<string> GenerateSearchPatterns(string term)
    {
        var patterns = new List<string>();
        
        // Adiciona o termo original
        patterns.Add(term);
        
        // Adiciona versão normalizada (sem hífens/espaços)
        var normalized = NormalizeHymnNumber(term);
        if (normalized != term.ToUpperInvariant())
        {
            patterns.Add(normalized);
        }
        
        // Adiciona versões com diferentes separadores
        if (term.Contains("-"))
        {
            patterns.Add(term.Replace("-", ""));
            patterns.Add(term.Replace("-", " "));
        }
        if (term.Contains(" "))
        {
            patterns.Add(term.Replace(" ", ""));
            patterns.Add(term.Replace(" ", "-"));
        }
        
        // Remove duplicatas e retorna
        return patterns.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public HinarioSqliteService(ILogger<HinarioSqliteService>? logger = null)
    {
        // Caminho do arquivo SQLite
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var currentDir = Directory.GetCurrentDirectory();
        var baseDir = AppContext.BaseDirectory;
        
        var possiblePaths = new[]
        {
            // Docker: /app/data/Hinario/HinarioCompleto.sqlite
            Path.Combine(baseDir, "data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(currentDir, "data", "Hinario", "HinarioCompleto.sqlite"),
            // Desenvolvimento local: Data/Hinario/HinarioCompleto.sqlite
            Path.Combine(projectRoot, "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(currentDir, "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(baseDir, "Data", "Hinario", "HinarioCompleto.sqlite")
        };

        _dbPath = possiblePaths.FirstOrDefault(File.Exists) ?? possiblePaths.First();
        _logger = logger;
        
        // Log do caminho encontrado para debug
        _logger?.LogInformation("HinarioSqliteService inicializado. Caminho do banco: {DbPath}, Existe: {Exists}", _dbPath, File.Exists(_dbPath));
    }

    private string GetConnectionString()
    {
        return $"Data Source={_dbPath};Mode=ReadOnly";
    }

    private bool IsSqliteAvailable()
    {
        return File.Exists(_dbPath);
    }

    public async Task<List<HymnResponseDto>> GetAllAsync()
    {
        if (!IsSqliteAvailable())
        {
            _logger?.LogWarning("Arquivo SQLite não encontrado: {DbPath}", _dbPath);
            return new List<HymnResponseDto>();
        }

        try
        {
            using var connection = new SqliteConnection(GetConnectionString());
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                FROM ZENTITY 
                ORDER BY ZNUMERO ASC";

            var hymns = new List<HymnResponseDto>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var hymn = MapRowToHymnDto(reader);
                if (hymn != null)
                {
                    hymns.Add(hymn);
                }
            }

            _logger?.LogInformation("Lidos {Count} hinos do SQLite", hymns.Count);
            return hymns;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao ler hinos do SQLite");
            return new List<HymnResponseDto>();
        }
    }

    public async Task<HymnResponseDto?> GetByNumberAsync(string number)
    {
        if (!IsSqliteAvailable())
        {
            return null;
        }

        try
        {
            using var connection = new SqliteConnection(GetConnectionString());
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                FROM ZENTITY 
                WHERE ZNUMERO = @number
                LIMIT 1";
            
            command.Parameters.AddWithValue("@number", number);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapRowToHymnDto(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar hino {Number} do SQLite", number);
            return null;
        }
    }

    public async Task<HymnResponseDto?> GetByIdAsync(int id)
    {
        if (!IsSqliteAvailable())
        {
            return null;
        }

        try
        {
            using var connection = new SqliteConnection(GetConnectionString());
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                FROM ZENTITY 
                WHERE Z_PK = @id
                LIMIT 1";
            
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapRowToHymnDto(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar hino com ID {Id} do SQLite", id);
            return null;
        }
    }

    public async Task<List<HymnResponseDto>> SearchAsync(string term)
    {
        if (!IsSqliteAvailable())
        {
            _logger?.LogWarning("SQLite não disponível para busca com termo: {Term}", term);
            return new List<HymnResponseDto>();
        }

        try
        {
            using var connection = new SqliteConnection(GetConnectionString());
            await connection.OpenAsync();

            var normalizedTerm = NormalizeHymnNumber(term);
            var command = connection.CreateCommand();
            
            _logger?.LogInformation("Busca SQLite - Termo: '{Term}', Normalizado: '{Normalized}'", term, normalizedTerm);
            
            // Estratégia: busca ampla que captura candidatos e depois filtra em memória
            // Para garantir que encontre mesmo quando o formato difere (ex: "S-38" vs "S38")
            // Se o termo normalizado começa com letra seguida de número (ex: "S38" de "S-38"),
            // busca TODOS os hinos que começam com essa letra para o filtro em memória processar
            var firstChar = normalizedTerm.Length > 0 ? normalizedTerm[0].ToString() : "";
            var hasNumber = normalizedTerm.Length > 1 && char.IsDigit(normalizedTerm[1]);
            var isLetter = firstChar.Length > 0 && char.IsLetter(firstChar[0]);
            
            _logger?.LogInformation("Detecção: firstChar='{FirstChar}', hasNumber={HasNumber}, isLetter={IsLetter}", 
                firstChar, hasNumber, isLetter);
            
            // Se o termo normalizado começa com letra seguida de número, busca ampla por todos os hinos dessa letra
            if (hasNumber && isLetter)
            {
                var firstCharPattern = $"{firstChar}%";
                _logger?.LogInformation("Usando busca ampla com padrão: '{Pattern}'", firstCharPattern);
                
                // Busca ampla: todos os hinos que começam com a letra (ex: "S%") + busca normal
                command.CommandText = @"
                    SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                    FROM ZENTITY 
                    WHERE (
                        -- Busca no número: formato original
                        ZNUMERO LIKE @term COLLATE NOCASE
                        OR ZNUMERO = @term COLLATE NOCASE
                        -- Busca no número: formato normalizado
                        OR REPLACE(REPLACE(REPLACE(ZNUMERO, '-', ''), ' ', ''), '.', '') LIKE @termNorm COLLATE NOCASE
                        OR REPLACE(REPLACE(REPLACE(ZNUMERO, '-', ''), ' ', ''), '.', '') = @termNormExact COLLATE NOCASE
                        OR ZNUMERO LIKE @termNorm COLLATE NOCASE
                        OR ZNUMERO = @termNormExact COLLATE NOCASE
                        -- Busca ampla: todos os hinos que começam com a letra (para o filtro processar)
                        OR ZNUMERO LIKE @firstCharPattern COLLATE NOCASE
                    )
                    OR (
                        -- Busca em título e letras
                        ZTITULO LIKE @term COLLATE NOCASE
                        OR ZLETRA LIKE @term COLLATE NOCASE
                        OR ZTITULO LIKE @termNorm COLLATE NOCASE
                        OR ZLETRA LIKE @termNorm COLLATE NOCASE
                    )
                    ORDER BY ZNUMERO ASC";
                
                command.Parameters.AddWithValue("@firstCharPattern", firstCharPattern);
                _logger?.LogInformation("Query SQL configurada com busca ampla para padrão '{Pattern}'", firstCharPattern);
            }
            else
            {
                _logger?.LogInformation("Usando busca normal (não atendeu critérios para busca ampla)");
                // Busca normal para outros casos
                command.CommandText = @"
                    SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                    FROM ZENTITY 
                    WHERE (
                        ZNUMERO LIKE @term COLLATE NOCASE
                        OR ZNUMERO = @term COLLATE NOCASE
                        OR REPLACE(REPLACE(REPLACE(ZNUMERO, '-', ''), ' ', ''), '.', '') LIKE @termNorm COLLATE NOCASE
                        OR REPLACE(REPLACE(REPLACE(ZNUMERO, '-', ''), ' ', ''), '.', '') = @termNormExact COLLATE NOCASE
                        OR ZNUMERO LIKE @termNorm COLLATE NOCASE
                        OR ZNUMERO = @termNormExact COLLATE NOCASE
                    )
                    OR (
                        ZTITULO LIKE @term COLLATE NOCASE
                        OR ZLETRA LIKE @term COLLATE NOCASE
                        OR ZTITULO LIKE @termNorm COLLATE NOCASE
                        OR ZLETRA LIKE @termNorm COLLATE NOCASE
                    )
                    ORDER BY ZNUMERO ASC";
            }

            var termPattern = $"%{term}%";
            var normPattern = $"%{normalizedTerm}%";
            
            command.Parameters.AddWithValue("@term", termPattern);
            command.Parameters.AddWithValue("@termNorm", normPattern);
            command.Parameters.AddWithValue("@termNormExact", normalizedTerm);

            var allHymns = new List<HymnResponseDto>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var hymn = MapRowToHymnDto(reader);
                if (hymn != null)
                {
                    allHymns.Add(hymn);
                }
            }
            
            // Filtro adicional em memória para garantir que encontre mesmo com diferentes formatos
            // Compara o número normalizado do hino com o termo normalizado
            var filteredHymns = allHymns.Where(h =>
            {
                var normalizedHymnNumber = NormalizeHymnNumber(h.Number);
                // Compara número normalizado
                if (normalizedHymnNumber.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase) ||
                    normalizedHymnNumber == normalizedTerm)
                {
                    return true;
                }
                // Compara número original
                if (h.Number.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    h.Number == term)
                {
                    return true;
                }
                // Compara título e letras
                if (h.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    h.Verses.Any(v => v.Lines.Any(l => l.Contains(term, StringComparison.OrdinalIgnoreCase))))
                {
                    return true;
                }
                return false;
            }).ToList();

            _logger?.LogInformation("Busca SQLite com termo '{Term}' (normalizado: '{Normalized}'): {SqlCount} da query, {FilteredCount} após filtro", term, normalizedTerm, allHymns.Count, filteredHymns.Count);
            
            // Log detalhado para debug
            if (allHymns.Count > 0 && filteredHymns.Count == 0)
            {
                _logger?.LogWarning("Query retornou {Count} hinos mas filtro não encontrou nenhum. Exemplos: {Examples}", 
                    allHymns.Count, 
                    string.Join(", ", allHymns.Take(3).Select(h => $"{h.Number} (normalizado: {NormalizeHymnNumber(h.Number)})")));
            }
            
            return filteredHymns;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar hinos no SQLite com termo {Term}", term);
            return new List<HymnResponseDto>();
        }
    }

    public async Task<List<HymnResponseDto>> FilterByCategoryAsync(string category)
    {
        // Como o SQLite não tem categoria explícita, vamos filtrar por número
        // C = Canticos, números começam com C
        // H = Hinario, números são apenas numéricos
        // S = Suplementar, números começam com S
        
        if (!IsSqliteAvailable())
        {
            return new List<HymnResponseDto>();
        }

        try
        {
            using var connection = new SqliteConnection(GetConnectionString());
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            string whereClause = category.ToLower() switch
            {
                "canticos" => "ZNUMERO LIKE 'C%'",
                "suplementar" => "ZNUMERO LIKE 'S%'",
                "hinario" => "ZNUMERO NOT LIKE 'C%' AND ZNUMERO NOT LIKE 'S%' AND ZNUMERO GLOB '[0-9]*'",
                "novos" => "ZNUMERO LIKE 'N%' OR ZNUMERO LIKE 'New%'",
                _ => "1=1" // Todas as categorias
            };

            command.CommandText = $@"
                SELECT Z_PK, ZNUMERO, ZTITULO, ZLETRA 
                FROM ZENTITY 
                WHERE {whereClause}
                ORDER BY ZNUMERO ASC";

            var hymns = new List<HymnResponseDto>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var hymn = MapRowToHymnDto(reader);
                if (hymn != null)
                {
                    hymns.Add(hymn);
                }
            }

            return hymns;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao filtrar hinos por categoria {Category} no SQLite", category);
            return new List<HymnResponseDto>();
        }
    }

    private HymnResponseDto? MapRowToHymnDto(SqliteDataReader reader)
    {
        try
        {
            var id = reader.GetInt32(reader.GetOrdinal("Z_PK"));
            var numero = reader.IsDBNull(reader.GetOrdinal("ZNUMERO")) 
                ? string.Empty 
                : reader.GetString(reader.GetOrdinal("ZNUMERO"));
            var titulo = reader.IsDBNull(reader.GetOrdinal("ZTITULO")) 
                ? string.Empty 
                : reader.GetString(reader.GetOrdinal("ZTITULO"));
            var letra = reader.IsDBNull(reader.GetOrdinal("ZLETRA")) 
                ? string.Empty 
                : reader.GetString(reader.GetOrdinal("ZLETRA"));

            // Determinar categoria baseado no número
            var category = DetermineCategory(numero);

            // Processar letra para extrair versos
            var verses = ParseLyricsToVerses(letra);

            return new HymnResponseDto
            {
                Id = -id, // ID negativo para diferenciar do PostgreSQL
                Number = numero,
                Title = titulo,
                Category = category,
                HymnBook = "EAV - Editora Árvore da Vida",
                Key = null,
                Bpm = null,
                Verses = verses
            };
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao mapear linha do SQLite para DTO");
            return null;
        }
    }

    private string DetermineCategory(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return "hinario";

        if (number.StartsWith("C", StringComparison.OrdinalIgnoreCase))
            return "canticos";
        
        if (number.StartsWith("S", StringComparison.OrdinalIgnoreCase))
            return "suplementar";
        
        if (number.StartsWith("N", StringComparison.OrdinalIgnoreCase))
            return "novos";

        // Se é apenas numérico, é do hinário principal
        if (Regex.IsMatch(number, @"^\d+$"))
            return "hinario";

        return "hinario";
    }

    private List<VerseDto> ParseLyricsToVerses(string lyrics)
    {
        if (string.IsNullOrWhiteSpace(lyrics))
            return new List<VerseDto>();

        var verses = new List<VerseDto>();
        var lines = lyrics.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        VerseDto? currentVerse = null;
        var currentLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                if (currentVerse != null && currentLines.Count > 0)
                {
                    currentVerse.Lines = currentLines;
                    verses.Add(currentVerse);
                    currentVerse = null;
                    currentLines = new List<string>();
                }
                continue;
            }

            // Detectar início de verso de forma mais conservadora para evitar perda de texto
            // Padrões válidos: 
            // - Número seguido de espaço/pontuação (ex: "1 ", "1.", "1-")
            // - V seguido de número (ex: "V1", "v1")
            // - R, C ou P sozinhos seguidos de espaço/pontuação (ex: "R ", "C.", "P-")
            bool isVerseMarker = false;
            string verseTypeStr = "";
            string remainingText = trimmedLine;
            
            // Padrão 1: Número no início seguido de espaço, ponto, hífen ou dois pontos
            var numberMatch = Regex.Match(trimmedLine, @"^(\d+)(\s|\.|-|:)(.*)$");
            if (numberMatch.Success)
            {
                verseTypeStr = $"V{numberMatch.Groups[1].Value}";
                remainingText = numberMatch.Groups[3].Value.Trim();
                isVerseMarker = true;
            }
            // Padrão 2: V seguido de número (ex: "V1", "v1")
            else if (Regex.IsMatch(trimmedLine, @"^[Vv]\d+", RegexOptions.IgnoreCase))
            {
                var vMatch = Regex.Match(trimmedLine, @"^[Vv](\d+)(?:\s*[\.\-:]?\s*)(.*)$", RegexOptions.IgnoreCase);
                if (vMatch.Success)
                {
                    verseTypeStr = $"V{vMatch.Groups[1].Value}";
                    remainingText = vMatch.Groups[2].Value.Trim();
                    isVerseMarker = true;
                }
            }
            // Padrão 3: R, C ou P sozinhos seguidos de espaço, ponto, hífen ou dois pontos (não parte de palavra)
            else if (Regex.IsMatch(trimmedLine, @"^[RCP](?:\s|\.|-|:|\s*$)", RegexOptions.IgnoreCase))
            {
                var markerMatch = Regex.Match(trimmedLine, @"^([RCP])(?:\s|\.|-|:|\s*)(.*)$", RegexOptions.IgnoreCase);
                if (markerMatch.Success)
                {
                    verseTypeStr = markerMatch.Groups[1].Value.ToUpper();
                    remainingText = markerMatch.Groups[2].Value.Trim();
                    isVerseMarker = true;
                }
            }
            
            if (isVerseMarker)
            {
                // Salvar verso anterior se existir
                if (currentVerse != null && currentLines.Count > 0)
                {
                    currentVerse.Lines = currentLines;
                    verses.Add(currentVerse);
                }

                // Criar novo verso
                var verseType = DetermineVerseType(verseTypeStr);
                
                currentVerse = new VerseDto { Type = verseType };
                currentLines = string.IsNullOrWhiteSpace(remainingText) 
                    ? new List<string>() 
                    : new List<string> { remainingText };
            }
            else
            {
                // Continuar linha do verso atual
                if (currentVerse == null)
                {
                    // Se não há verso atual, criar um genérico
                    currentVerse = new VerseDto { Type = "V1" };
                }
                currentLines.Add(trimmedLine);
            }
        }

        // Adicionar último verso
        if (currentVerse != null && currentLines.Count > 0)
        {
            currentVerse.Lines = currentLines;
            verses.Add(currentVerse);
        }

        // Se não conseguiu parsear, criar um único verso com toda a letra
        if (verses.Count == 0 && !string.IsNullOrWhiteSpace(lyrics))
        {
            verses.Add(new VerseDto
            {
                Type = "V1",
                Lines = lines.Where(l => !string.IsNullOrWhiteSpace(l.Trim())).ToList()
            });
        }

        return verses;
    }

    private string DetermineVerseType(string indicator)
    {
        return indicator.ToUpper() switch
        {
            "R" => "R",
            "C" => "C",
            "P" => "P",
            "V" => "R",
            var num when int.TryParse(num, out _) => $"V{num}",
            _ => $"V{indicator}"
        };
    }
}


