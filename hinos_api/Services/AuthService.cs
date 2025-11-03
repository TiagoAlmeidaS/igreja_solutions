using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using hinos_api.DTOs;

namespace hinos_api.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponseDto? Authenticate(string email, string password)
    {
        var configuredEmail = _configuration["Auth:Email"] ?? string.Empty;
        var configuredPassword = _configuration["Auth:Password"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(configuredEmail) || string.IsNullOrWhiteSpace(configuredPassword))
        {
            return null;
        }

        if (email != configuredEmail || password != configuredPassword)
        {
            return null;
        }

        var token = GenerateJwtToken(email);
        var user = new UserDto
        {
            Id = "1",
            Name = "Administrador",
            Email = email
        };

        return new LoginResponseDto
        {
            Token = token,
            User = user
        };
    }

    private string GenerateJwtToken(string email)
    {
        var secret = _configuration["Auth:JwtSecret"] 
            ?? throw new InvalidOperationException("JWT Secret não configurado");
        
        var expirationHours = int.TryParse(_configuration["Auth:JwtExpirationHours"], out var hours) 
            ? hours 
            : 24;

        var key = Encoding.UTF8.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Administrador")
            }),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

