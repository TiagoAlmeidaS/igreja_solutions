using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.DTOs;

namespace hinos_api.Services;

public class HymnQueryService
{
    private readonly HymnsDbContext _dbContext;
    private readonly HinarioSqliteService _sqliteService;
    private readonly ILogger<HymnQueryService>? _logger;

    public HymnQueryService(
        HymnsDbContext dbContext,
        HinarioSqliteService sqliteService,
        ILogger<HymnQueryService>? logger = null)
    {
        _dbContext = dbContext;
        _sqliteService = sqliteService;
        _logger = logger;
    }

    public async Task<List<HymnResponseDto>> GetAllAsync(string? category = null, string? search = null)
    {
        var results = new List<HymnResponseDto>();

        // Buscar no SQLite primeiro
        try
        {
            List<HymnResponseDto> sqliteHymns;
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                sqliteHymns = await _sqliteService.FilterByCategoryAsync(category);
            }
            else if (!string.IsNullOrWhiteSpace(search))
            {
                sqliteHymns = await _sqliteService.SearchAsync(search);
            }
            else
            {
                sqliteHymns = await _sqliteService.GetAllAsync();
            }

            results.AddRange(sqliteHymns);
            _logger?.LogInformation("Encontrados {Count} hinos no SQLite", sqliteHymns.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao buscar hinos no SQLite, continuando apenas com PostgreSQL");
        }

        // Buscar no PostgreSQL
        try
        {
            var query = _dbContext.Hymns.Include(h => h.Verses).AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(h => h.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(h =>
                    h.Number.ToLower().Contains(searchLower) ||
                    h.Title.ToLower().Contains(searchLower) ||
                    h.HymnBook.ToLower().Contains(searchLower) ||
                    h.Verses.Any(v => v.LinesJson.ToLower().Contains(searchLower))
                );
            }

            var postgresHymns = await query.ToListAsync();
            var postgresDtos = postgresHymns.Select(HymnService.MapToDto).ToList();
            
            results.AddRange(postgresDtos);
            _logger?.LogInformation("Encontrados {Count} hinos no PostgreSQL", postgresDtos.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar hinos no PostgreSQL");
        }

        // Remover duplicatas (priorizar PostgreSQL - IDs positivos)
        var uniqueResults = results
            .GroupBy(h => h.Number, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(h => h.Id).First()) // ID positivo (PostgreSQL) primeiro
            .OrderBy(h => h.Number)
            .ToList();

        _logger?.LogInformation("Total de hinos únicos: {Count}", uniqueResults.Count);
        return uniqueResults;
    }

    public async Task<HymnResponseDto?> GetByIdAsync(int id)
    {
        // IDs positivos são do PostgreSQL
        if (id > 0)
        {
            try
            {
                var hymn = await _dbContext.Hymns
                    .Include(h => h.Verses)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (hymn != null)
                {
                    return HymnService.MapToDto(hymn);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Erro ao buscar hino {Id} no PostgreSQL", id);
            }
        }

        // IDs negativos são do SQLite (usar valor absoluto como referência)
        if (id < 0)
        {
            try
            {
                var sqliteId = Math.Abs(id);
                var hymn = await _sqliteService.GetByIdAsync(sqliteId);
                if (hymn != null)
                {
                    return hymn;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Erro ao buscar hino {Id} no SQLite", id);
            }
        }

        return null;
    }

    public async Task<HymnResponseDto?> GetByNumberAsync(string number)
    {
        // Primeiro tenta PostgreSQL
        var postgresHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Number == number);

        if (postgresHymn != null)
        {
            return HymnService.MapToDto(postgresHymn);
        }

        // Se não encontrou, tenta SQLite
        var sqliteHymn = await _sqliteService.GetByNumberAsync(number);
        return sqliteHymn;
    }

    public async Task<List<HymnResponseDto>> SearchAsync(string term)
    {
        var results = new List<HymnResponseDto>();

        // Buscar no SQLite
        try
        {
            var sqliteHymns = await _sqliteService.SearchAsync(term);
            results.AddRange(sqliteHymns);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao buscar no SQLite");
        }

        // Buscar no PostgreSQL
        try
        {
            var termLower = term.ToLower();
            var postgresHymns = await _dbContext.Hymns
                .Include(h => h.Verses)
                .Where(h =>
                    h.Number.ToLower().Contains(termLower) ||
                    h.Number.ToLower() == termLower ||
                    h.Title.ToLower().Contains(termLower) ||
                    h.HymnBook.ToLower().Contains(termLower) ||
                    h.Verses.Any(v => v.LinesJson.ToLower().Contains(termLower))
                )
                .ToListAsync();

            var postgresDtos = postgresHymns.Select(HymnService.MapToDto);
            results.AddRange(postgresDtos);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar no PostgreSQL");
        }

        // Remover duplicatas
        var uniqueResults = results
            .GroupBy(h => h.Number, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(h => h.Id).First())
            .ToList();

        return uniqueResults;
    }
}

