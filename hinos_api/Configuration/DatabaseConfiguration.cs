using Microsoft.EntityFrameworkCore;
using hinos_api.Data;

namespace hinos_api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment? environment = null)
    {
        var envName = environment?.EnvironmentName 
            ?? configuration["ASPNETCORE_ENVIRONMENT"] 
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
            ?? "Development";

        // Em ambiente de teste, usar InMemory
        if (envName == "Testing")
        {
            services.AddDbContext<HymnsDbContext>(options =>
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
            return services;
        }

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada");

        // Configurar Entity Framework Core com PostgreSQL
        services.AddDbContext<HymnsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => 
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null)));

        return services;
    }
}

