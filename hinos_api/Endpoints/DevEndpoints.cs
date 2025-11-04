using hinos_api.Scripts;

namespace hinos_api.Endpoints;

public static class DevEndpoints
{
    public static WebApplication MapDevEndpoints(this WebApplication app)
    {
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

        return app;
    }
}

