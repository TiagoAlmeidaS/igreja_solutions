using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using hinos_api.Data;
using hinos_api.DTOs;
using hinos_api.Models;
using hinos_api.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace hinos_api.Tests.Integration;

public class HymnsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly HymnsDbContext _dbContext;
    private readonly IServiceScope _scope;

    public HymnsApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        // Obter DbContext para setup e assertions
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<HymnsDbContext>();
        _dbContext.Database.EnsureCreated();
    }

    #region GET /api/hymns

    [Fact]
    public async Task GetHymns_ShouldReturnEmptyList_WhenNoHymnsExist()
    {
        // Act
        var response = await _client.GetAsync("/api/hymns");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hymns = await response.Content.ReadFromJsonAsync<List<HymnResponseDto>>();
        hymns.Should().NotBeNull();
        hymns.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHymns_ShouldReturnAllHymns_WhenHymnsExist()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Number = "101";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Number = "102";
        
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/hymns");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hymns = await response.Content.ReadFromJsonAsync<List<HymnResponseDto>>();
        hymns.Should().NotBeNull();
        hymns.Should().HaveCountGreaterThanOrEqualTo(2);
        hymns.Should().Contain(h => h.Number == "101");
        hymns.Should().Contain(h => h.Number == "102");
    }

    [Fact]
    public async Task GetHymns_ShouldFilterByCategory_WhenCategoryProvided()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Number = "201";
        hymn1.Category = "hinario";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Number = "C1";
        hymn2.Category = "canticos";
        
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/hymns?category=hinario");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hymns = await response.Content.ReadFromJsonAsync<List<HymnResponseDto>>();
        hymns.Should().NotBeNull();
        hymns.Should().Contain(h => h.Number == "201");
        hymns.Should().NotContain(h => h.Number == "C1");
        hymns.All(h => h.Category == "hinario").Should().BeTrue();
    }

    [Fact]
    public async Task GetHymns_ShouldFilterBySearch_WhenSearchTermProvided()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "301";
        hymn.Title = "Graça Maravilhosa";
        
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/hymns?search=Graça");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hymns = await response.Content.ReadFromJsonAsync<List<HymnResponseDto>>();
        hymns.Should().NotBeNull();
        hymns.Should().Contain(h => h.Title.Contains("Graça", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region GET /api/hymns/{id}

    [Fact]
    public async Task GetHymnById_ShouldReturnHymn_WhenHymnExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "401";
        hymn.Title = "Hino Teste";
        
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/hymns/{hymn.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedHymn = await response.Content.ReadFromJsonAsync<HymnResponseDto>();
        returnedHymn.Should().NotBeNull();
        returnedHymn!.Id.Should().Be(hymn.Id);
        returnedHymn.Number.Should().Be("401");
        returnedHymn.Title.Should().Be("Hino Teste");
    }

    [Fact]
    public async Task GetHymnById_ShouldReturn404_WhenHymnDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/hymns/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/hymns/search

    [Fact]
    public async Task SearchHymns_ShouldReturnBadRequest_WhenTermIsEmpty()
    {
        // Act
        var response = await _client.GetAsync("/api/hymns/search");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchHymns_ShouldReturnHymns_WhenTermMatches()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "501";
        hymn.Title = "Castelo Forte";
        
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/hymns/search?term=Castelo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hymns = await response.Content.ReadFromJsonAsync<List<HymnResponseDto>>();
        hymns.Should().NotBeNull();
        hymns.Should().Contain(h => h.Title.Contains("Castelo", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region POST /api/hymns

    [Fact]
    public async Task CreateHymn_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "601";
        dto.Title = "Novo Hino";
        dto.Category = "hinario";
        dto.HymnBook = "Hinário Teste";

        // Act
        var response = await _client.PostAsJsonAsync("/api/hymns", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdHymn = await response.Content.ReadFromJsonAsync<HymnResponseDto>();
        createdHymn.Should().NotBeNull();
        createdHymn!.Number.Should().Be("601");
        createdHymn.Title.Should().Be("Novo Hino");
        
        // Verificar se foi salvo no banco
        var savedHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == createdHymn.Id);
        savedHymn.Should().NotBeNull();
        savedHymn!.Number.Should().Be("601");
    }

    [Fact]
    public async Task CreateHymn_ShouldReturnBadRequest_WhenNumberIsEmpty()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "";

        // Act
        var response = await _client.PostAsJsonAsync("/api/hymns", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateHymn_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Title = "";

        // Act
        var response = await _client.PostAsJsonAsync("/api/hymns", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateHymn_ShouldReturnConflict_WhenNumberAlreadyExists()
    {
        // Arrange
        var existingHymn = HymnFaker.CreateFakeHymn();
        existingHymn.Number = "701";
        _dbContext.Hymns.Add(existingHymn);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "701";

        // Act
        var response = await _client.PostAsJsonAsync("/api/hymns", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region PUT /api/hymns/{id}

    [Fact]
    public async Task UpdateHymn_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "801";
        hymn.Title = "Título Original";
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Number = "801";
        dto.Title = "Título Atualizado";

        // Act
        var response = await _client.PutAsJsonAsync($"/api/hymns/{hymn.Id}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedHymn = await response.Content.ReadFromJsonAsync<HymnResponseDto>();
        updatedHymn.Should().NotBeNull();
        updatedHymn!.Title.Should().Be("Título Atualizado");
        
        // Verificar no banco
        var savedHymn = await _dbContext.Hymns.FindAsync(hymn.Id);
        savedHymn.Should().NotBeNull();
        savedHymn!.Title.Should().Be("Título Atualizado");
    }

    [Fact]
    public async Task UpdateHymn_ShouldReturnNotFound_WhenHymnDoesNotExist()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeUpdateHymnDto();

        // Act
        var response = await _client.PutAsJsonAsync("/api/hymns/99999", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateHymn_ShouldReturnConflict_WhenNumberIsUsedByAnotherHymn()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Number = "901";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Number = "902";
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Number = "901"; // Tentar usar o número do hymn1

        // Act
        var response = await _client.PutAsJsonAsync($"/api/hymns/{hymn2.Id}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region DELETE /api/hymns/{id}

    [Fact]
    public async Task DeleteHymn_ShouldReturnNoContent_WhenHymnExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "1001";
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/hymns/{hymn.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verificar se foi removido
        var hymnExists = await _dbContext.Hymns.AnyAsync(h => h.Id == hymn.Id);
        hymnExists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteHymn_ShouldReturnNotFound_WhenHymnDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/hymns/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHymn_ShouldCascadeDeleteVerses_WhenHymnIsDeleted()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "1101";
        hymn.Verses = HymnFaker.CreateFakeVerses(3);
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        var verseCountBefore = await _dbContext.Verses.CountAsync(v => v.HymnId == hymn.Id);
        verseCountBefore.Should().Be(3);

        // Act
        var response = await _client.DeleteAsync($"/api/hymns/{hymn.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verificar se versos foram removidos
        var versesExist = await _dbContext.Verses.AnyAsync(v => v.HymnId == hymn.Id);
        versesExist.Should().BeFalse();
    }

    #endregion

    #region GET /api/hymns/{id}/download/plain

    [Fact]
    public async Task DownloadHymnPlain_ShouldReturnFile_WhenHymnExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "1201";
        hymn.Title = "Hino para Download";
        hymn.Verses = HymnFaker.CreateFakeVerses(2);
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/hymns/{hymn.Id}/download/plain");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Hino para Download");
    }

    [Fact]
    public async Task DownloadHymnPlain_ShouldReturn404_WhenHymnDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/hymns/99999/download/plain");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/hymns/{id}/download/holyrics

    [Fact]
    public async Task DownloadHymnHolyrics_ShouldReturnFile_WhenHymnExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "1301";
        hymn.Title = "Hino Holyrics";
        hymn.Key = "G";
        hymn.Bpm = 72;
        hymn.Verses = HymnFaker.CreateFakeVerses(2);
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/hymns/{hymn.Id}/download/holyrics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("#1301");
        content.Should().Contain("Hino Holyrics");
        content.Should().Contain("[V1]");
    }

    [Fact]
    public async Task DownloadHymnHolyrics_ShouldReturn404_WhenHymnDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/hymns/99999/download/holyrics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /health

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        _scope.Dispose();
        _client.Dispose();
    }
}

