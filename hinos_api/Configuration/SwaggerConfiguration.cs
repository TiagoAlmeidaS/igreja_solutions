namespace hinos_api.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        // Adicionar Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
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

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
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
        else
        {
            // Habilitar Swagger também em produção (se necessário)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hinos API v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "Hinos API - Documentação";
            });
        }

        return app;
    }
}

