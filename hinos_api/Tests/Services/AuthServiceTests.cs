using FluentAssertions;
using hinos_api.DTOs;
using hinos_api.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace hinos_api.Tests.Services;

public class AuthServiceTests
{
    private IConfiguration CreateMockConfiguration(string? email = "admin@hinario.com", string? password = "admin123", string? jwtSecret = "test-secret-key-with-at-least-32-characters-for-jwt", int? expirationHours = 24)
    {
        var configDict = new Dictionary<string, string?>
        {
            { "Auth:Email", email },
            { "Auth:Password", password },
            { "Auth:JwtSecret", jwtSecret },
            { "Auth:JwtExpirationHours", expirationHours?.ToString() }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configDict!)
            .Build();
    }

    [Fact]
    public void Authenticate_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(email);
        result.User.Id.Should().Be("1");
        result.User.Name.Should().Be("Administrador");
    }

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenEmailIsIncorrect()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var authService = new AuthService(config);
        var email = "wrong@email.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "wrongpassword";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenEmailAndPasswordAreBothIncorrect()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var authService = new AuthService(config);
        var email = "wrong@email.com";
        var password = "wrongpassword";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenCredentialsAreNotConfigured()
    {
        // Arrange
        var config = CreateMockConfiguration(email: null, password: null);
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Authenticate_ShouldGenerateValidJwtToken_WhenCredentialsAreValid()
    {
        // Arrange
        var config = CreateMockConfiguration();
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        
        // Validar que é um JWT válido
        var tokenHandler = new JwtSecurityTokenHandler();
        var canRead = tokenHandler.CanReadToken(result.Token);
        canRead.Should().BeTrue();

        var token = tokenHandler.ReadJwtToken(result.Token);
        token.Claims.Should().Contain(c => c.Type == "email" && c.Value == email);
        token.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == "1");
        token.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == "Administrador");
    }

    [Fact]
    public void Authenticate_ShouldUseCustomExpirationHours_WhenConfigured()
    {
        // Arrange
        var config = CreateMockConfiguration(expirationHours: 48);
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);
        
        // Verificar que a expiração está aproximadamente 48 horas no futuro (com tolerância)
        var expectedExpiration = DateTime.UtcNow.AddHours(48);
        token.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Authenticate_ShouldUseDefaultExpirationHours_WhenNotConfigured()
    {
        // Arrange
        var config = CreateMockConfiguration(expirationHours: null);
        var authService = new AuthService(config);
        var email = "admin@hinario.com";
        var password = "admin123";

        // Act
        var result = authService.Authenticate(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result.Token);
        
        // Verificar que a expiração está aproximadamente 24 horas no futuro (padrão)
        var expectedExpiration = DateTime.UtcNow.AddHours(24);
        token.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Authenticate_ShouldReturnCaseSensitiveCredentials_WhenCredentialsHaveDifferentCase()
    {
        // Arrange
        var config = CreateMockConfiguration(email: "Admin@Hinario.com", password: "Admin123");
        var authService = new AuthService(config);
        
        // Teste com email em minúsculas
        var result1 = authService.Authenticate("admin@hinario.com", "Admin123");
        
        // Teste com email correto
        var result2 = authService.Authenticate("Admin@Hinario.com", "Admin123");

        // Assert
        result1.Should().BeNull();
        result2.Should().NotBeNull();
    }
}

