using Microsoft.EntityFrameworkCore;
using hinos_api.Data;

namespace hinos_api.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
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

            // Popular dados iniciais apenas se o banco estiver vazio
            DbSeeder.Seed(db);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inicializar banco de dados");
            throw;
        }
    }
}

