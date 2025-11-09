using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using hinos_api.Data;

namespace hinos_api.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<global::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            // Adicionar configuração de teste com connection string fake
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Host=localhost;Port=5432;Database=test_db;Username=test;Password=test" },
                { "ASPNETCORE_ENVIRONMENT", "Testing" }
            });
        });
    }
}

