namespace hinos_api.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
    {
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthChecks("/health");

        return app;
    }
}

