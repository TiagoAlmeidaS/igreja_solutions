using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using hinos_api.Data;
using hinos_api.Services;
using hinos_api.DTOs;
using hinos_api.Models;
using hinos_api.Scripts;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework Core com PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var sqliteConnectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

// Determinar qual provider usar baseado na connection string
if (connectionString != null && connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
{
    // PostgreSQL
    builder.Services.AddDbContext<HymnsDbContext>(options =>
        options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));
}
else
{
    // SQLite (fallback)
    builder.Services.AddDbContext<HymnsDbContext>(options =>
        options.UseSqlite(connectionString ?? "Data Source=data/hymns.db"));
}

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
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    { 
        Title = "Hinos API", 
        Version = "v1",
        Description = "API REST para gerenciamento completo de hinários. Fornece endpoints para operações CRUD sobre hinos e versos, com suporte a filtros, busca e categorização.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Igreja Solutions",
            Email = "contato@igreja.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // Adicionar comentários XML (se existirem)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Adicionar health checks
builder.Services.AddHealthChecks();

// Registrar AuthService
builder.Services.AddScoped<AuthService>();

// Configurar autenticação JWT
var jwtSecret = builder.Configuration["Auth:JwtSecret"] 
    ?? throw new InvalidOperationException("JWT Secret não configurado");

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hinos API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Hinos API - Documentação";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableValidator();
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    });
}

// Habilitar Swagger também em produção (se necessário)
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hinos API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Hinos API - Documentação";
    });
}

app.UseCors();

// Adicionar autenticação e autorização ao pipeline
app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/health");

// Garantir que o diretório de dados existe
var dataDir = Path.Combine(app.Environment.ContentRootPath, "data");
if (!Directory.Exists(dataDir))
{
    Directory.CreateDirectory(dataDir);
}

// Aplicar migrations automaticamente, migrar dados e popular dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HymnsDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Aguardar até o banco estar disponível (especialmente importante para PostgreSQL no Docker)
        var maxRetries = 30;
        var retryCount = 0;
        while (retryCount < maxRetries)
        {
            try
            {
                if (await db.Database.CanConnectAsync())
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Tentativa {retryCount + 1}/{maxRetries}: Aguardando banco de dados... {ex.Message}");
                await Task.Delay(2000);
                retryCount++;
            }
        }

        // Criar banco e tabelas
        await db.Database.EnsureCreatedAsync();
        logger.LogInformation("Banco de dados inicializado com sucesso.");

        // Se estiver usando PostgreSQL e existe connection string SQLite, tentar migrar dados
        var isPostgres = connectionString != null && connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase);
        if (isPostgres && !string.IsNullOrEmpty(sqliteConnectionString))
        {
            var migrator = new SqliteToPostgresMigrator(db, sqliteConnectionString, 
                scope.ServiceProvider.GetRequiredService<ILogger<SqliteToPostgresMigrator>>());
            await migrator.MigrateAsync();
        }

        // Popular dados iniciais apenas se o banco estiver vazio
        DbSeeder.Seed(db);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar banco de dados");
        throw;
    }
}

// Endpoints da API

// POST /api/auth/login - Login de autenticação
app.MapPost("/api/auth/login", async (LoginRequestDto request, AuthService authService) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { message = "Email e senha são obrigatórios" });
    }

    var result = authService.Authenticate(request.Email, request.Password);
    
    if (result == null)
    {
        return Results.Json(new { message = "Credenciais inválidas" }, statusCode: StatusCodes.Status401Unauthorized);
    }

    return Results.Ok(result);
})
.WithName("Login")
.WithTags("Auth")
.WithSummary("Realiza login na API")
.WithDescription("Autentica um usuário usando email e senha. Retorna um token JWT e os dados do usuário autenticado.")
.Produces<LoginResponseDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status400BadRequest)
.AllowAnonymous();

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
.WithSummary("Lista todos os hinos")
.WithDescription("Retorna uma lista de todos os hinos cadastrados. Permite filtros opcionais por categoria e busca por termo (número, título, hinário ou conteúdo dos versos). Query parameters: category (hinario, canticos, suplementar, novos) e search (termo de busca).")
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
.WithSummary("Busca um hino por ID")
.WithDescription("Retorna os detalhes completos de um hino específico, incluindo todos os seus versos, com base no ID fornecido.")
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
.WithSummary("Busca hinos por termo")
.WithDescription("Realiza uma busca completa por termo nos hinos, pesquisando em número, título, hinário e conteúdo dos versos. Query parameter obrigatório: term (termo de busca).")
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
.WithSummary("Cria um novo hino")
.WithDescription("Cria um novo hino no sistema. O número do hino deve ser único. Campos obrigatórios: Number, Title, Category, HymnBook. Verses são opcionais.")
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
.WithSummary("Atualiza um hino existente")
.WithDescription("Atualiza os dados de um hino existente. O número do hino deve ser único e não pode estar sendo usado por outro hino. Os versos serão completamente substituídos.")
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
.WithSummary("Remove um hino")
.WithDescription("Remove um hino do sistema. Todos os versos associados serão removidos automaticamente (cascade delete).")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// Endpoint para análise do banco Hinario (apenas desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/dev/analyze-hinario", () =>
    {
        var hinarioDbPath = Path.Combine(app.Environment.ContentRootPath, "Data", "Hinario", "HinarioCompleto.sqlite");
        var analysis = HinarioDbAnalyzer.Analyze(hinarioDbPath);
        return Results.Text(analysis, "text/plain");
    })
    .WithName("AnalyzeHinarioDb")
    .WithTags("Development")
    .ExcludeFromDescription();
}

app.Run();

// Expor Program para testes
public partial class Program { }
