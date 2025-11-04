using Microsoft.AspNetCore.Mvc;
using hinos_api.DTOs;
using hinos_api.Services;

namespace hinos_api.Endpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        // POST /api/auth/login - Login de autenticação
        app.MapPost("/api/auth/login", async ([FromBody] LoginRequestDto request, [FromServices] AuthService authService) =>
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

        return app;
    }
}

