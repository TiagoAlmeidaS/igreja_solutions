using Microsoft.Data.Sqlite;
using hinos_api.DTOs;
using System.Text.RegularExpressions;

namespace hinos_api.Services;

public class HinarioSqliteService
{
    private readonly string _dbPath;
    private readonly ILogger<HinarioSqliteService>? _logger;

    public HinarioSqliteService(ILogger<HinarioSqliteService>? logger = null)
    {
        // Caminho do arquivo SQLite
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var possiblePaths = new[]
        {
            Path.Combine(projectRoot, "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "Hinario", "HinarioCompleto.sqlite"),
            Path.Combine(AppContext.BaseDirectory, "Data", "Hinario", "HinarioCompleto.sqlite")
        };

        _dbPath = possiblePaths.FirstOrDefault(File.Exists) ?? possiblePaths.First();
        _logger = logger;
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

    public async Task<List<HymnResponseDto>> SearchAsync(string term)
    {
        if (!IsSqliteAvailable())
        {
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
                WHERE ZNUMERO LIKE @term 
                   OR ZTITULO LIKE @term 
                   OR ZLETRA LIKE @term
                ORDER BY ZNUMERO ASC";

            command.Parameters.AddWithValue("@term", $"%{term}%");

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

            // Detectar início de verso (V1, V2, R, C, etc.)
            var verseMatch = Regex.Match(trimmedLine, @"^(\d+|[VRCP])\s*[\.\-]?\s*(.*)$", RegexOptions.IgnoreCase);
            if (verseMatch.Success)
            {
                // Salvar verso anterior se existir
                if (currentVerse != null && currentLines.Count > 0)
                {
                    currentVerse.Lines = currentLines;
                    verses.Add(currentVerse);
                }

                // Criar novo verso
                var verseType = DetermineVerseType(verseMatch.Groups[1].Value);
                var verseText = verseMatch.Groups[2].Value.Trim();
                
                currentVerse = new VerseDto { Type = verseType };
                currentLines = string.IsNullOrWhiteSpace(verseText) 
                    ? new List<string>() 
                    : new List<string> { verseText };
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


