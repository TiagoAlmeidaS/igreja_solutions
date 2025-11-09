using hinos_api.Configuration;
using hinos_api.Data;
using hinos_api.Endpoints;
using hinos_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurações de serviços
builder.Services.AddDatabaseConfiguration(builder.Configuration, builder.Environment);
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHealthChecks();
builder.Services.AddJwtConfiguration(builder.Configuration);

// Registrar serviços
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HinarioSqliteService>();
builder.Services.AddScoped<HymnQueryService>();
builder.Services.AddScoped<HymnFormatService>();

var app = builder.Build();

// Configurar pipeline de middleware
app.UseMiddlewareConfiguration();
app.UseSwaggerConfiguration();

// // Inicializar banco de dados
await DatabaseInitializer.InitializeDatabaseAsync(app);

// Mapear endpoints
app.MapAuthEndpoints();
app.MapHymnsEndpoints();
app.MapDevEndpoints();

app.Run();

// Expor Program para testes
public partial class Program { }
