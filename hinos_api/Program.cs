using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.Services;
using hinos_api.DTOs;
using hinos_api.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework Core com SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HymnsDbContext>(options =>
    options.UseSqlite(connectionString));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "http://localhost:4173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Adicionar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Hinos API", 
        Version = "v1",
        Description = "API para gerenciamento de hinários"
    });
});

// Adicionar health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHealthChecks("/health");

// Garantir que o diretório de dados existe
var dataDir = Path.Combine(app.Environment.ContentRootPath, "data");
if (!Directory.Exists(dataDir))
{
    Directory.CreateDirectory(dataDir);
}

// Aplicar migrations automaticamente e popular dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HymnsDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

// Endpoints da API

// GET /api/hymns - Listar todos os hinos (com filtros opcionais)
app.MapGet("/api/hymns", async (HymnsDbContext db, string? category, string? search) =>
{
    var query = db.Hymns.Include(h => h.Verses).AsQueryable();

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

    var hymns = await query.ToListAsync();
    return Results.Ok(hymns.Select(HymnService.MapToDto));
})
.WithName("GetHymns")
.WithTags("Hymns")
.Produces<List<HymnResponseDto>>(StatusCodes.Status200OK);

// GET /api/hymns/{id} - Buscar hino por ID
app.MapGet("/api/hymns/{id:int}", async (HymnsDbContext db, int id) =>
{
    var hymn = await db.Hymns
        .Include(h => h.Verses)
        .FirstOrDefaultAsync(h => h.Id == id);

    if (hymn == null)
    {
        return Results.NotFound(new { message = "Hino não encontrado" });
    }

    return Results.Ok(HymnService.MapToDto(hymn));
})
.WithName("GetHymnById")
.WithTags("Hymns")
.Produces<HymnResponseDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// GET /api/hymns/search?term={term} - Buscar por termo
app.MapGet("/api/hymns/search", async (HymnsDbContext db, string term) =>
{
    if (string.IsNullOrWhiteSpace(term))
    {
        return Results.BadRequest(new { message = "O termo de busca é obrigatório" });
    }

    var termLower = term.ToLower();
    var hymns = await db.Hymns
        .Include(h => h.Verses)
        .Where(h =>
            h.Number.ToLower().Contains(termLower) ||
            h.Number.ToLower() == termLower ||
            h.Title.ToLower().Contains(termLower) ||
            h.HymnBook.ToLower().Contains(termLower) ||
            h.Verses.Any(v => v.LinesJson.ToLower().Contains(termLower))
        )
        .ToListAsync();

    return Results.Ok(hymns.Select(HymnService.MapToDto));
})
.WithName("SearchHymns")
.WithTags("Hymns")
.Produces<List<HymnResponseDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

// POST /api/hymns - Criar novo hino
app.MapPost("/api/hymns", async (HymnsDbContext db, CreateHymnDto dto) =>
{
    // Validações básicas
    if (string.IsNullOrWhiteSpace(dto.Number))
        return Results.BadRequest(new { message = "O número do hino é obrigatório" });
    
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { message = "O título do hino é obrigatório" });

    if (string.IsNullOrWhiteSpace(dto.Category))
        return Results.BadRequest(new { message = "A categoria é obrigatória" });

    // Verificar se já existe um hino com o mesmo número
    var exists = await db.Hymns.AnyAsync(h => h.Number == dto.Number);
    if (exists)
    {
        return Results.Conflict(new { message = $"Já existe um hino com o número {dto.Number}" });
    }

    var hymn = HymnService.MapFromCreateDto(dto);
    db.Hymns.Add(hymn);
    await db.SaveChangesAsync();

    // Atualizar HymnId nos versos após salvar
    foreach (var verse in hymn.Verses)
    {
        verse.HymnId = hymn.Id;
    }
    await db.SaveChangesAsync();

    // Recarregar com versos
    var savedHymn = await db.Hymns
        .Include(h => h.Verses)
        .FirstAsync(h => h.Id == hymn.Id);

    return Results.Created($"/api/hymns/{savedHymn.Id}", HymnService.MapToDto(savedHymn));
})
.WithName("CreateHymn")
.WithTags("Hymns")
.Produces<HymnResponseDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict);

// PUT /api/hymns/{id} - Atualizar hino
app.MapPut("/api/hymns/{id:int}", async (HymnsDbContext db, int id, UpdateHymnDto dto) =>
{
    var hymn = await db.Hymns
        .Include(h => h.Verses)
        .FirstOrDefaultAsync(h => h.Id == id);

    if (hymn == null)
    {
        return Results.NotFound(new { message = "Hino não encontrado" });
    }

    // Validações básicas
    if (string.IsNullOrWhiteSpace(dto.Number))
        return Results.BadRequest(new { message = "O número do hino é obrigatório" });
    
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { message = "O título do hino é obrigatório" });

    // Verificar se o número já está sendo usado por outro hino
    if (dto.Number != hymn.Number)
    {
        var exists = await db.Hymns.AnyAsync(h => h.Number == dto.Number && h.Id != id);
        if (exists)
        {
            return Results.Conflict(new { message = $"Já existe um hino com o número {dto.Number}" });
        }
    }

    HymnService.UpdateFromDto(hymn, dto);
    
    // Atualizar HymnId nos versos
    foreach (var verse in hymn.Verses)
    {
        verse.HymnId = hymn.Id;
    }

    await db.SaveChangesAsync();

    // Recarregar com versos
    var updatedHymn = await db.Hymns
        .Include(h => h.Verses)
        .FirstAsync(h => h.Id == id);

    return Results.Ok(HymnService.MapToDto(updatedHymn));
})
.WithName("UpdateHymn")
.WithTags("Hymns")
.Produces<HymnResponseDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status409Conflict);

// DELETE /api/hymns/{id} - Deletar hino
app.MapDelete("/api/hymns/{id:int}", async (HymnsDbContext db, int id) =>
{
    var hymn = await db.Hymns
        .Include(h => h.Verses)
        .FirstOrDefaultAsync(h => h.Id == id);

    if (hymn == null)
    {
        return Results.NotFound(new { message = "Hino não encontrado" });
    }

    db.Hymns.Remove(hymn);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteHymn")
.WithTags("Hymns")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();

// Expor Program para testes
public partial class Program { }
