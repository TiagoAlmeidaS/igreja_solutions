using FluentAssertions;
using hinos_api.Data;
using hinos_api.DTOs;
using hinos_api.Models;
using hinos_api.Services;
using hinos_api.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace hinos_api.Tests.Services;

public class WarCryServiceTests
{
    private readonly WarCryService _service;
    private readonly HymnsDbContext _context;

    public WarCryServiceTests()
    {
        _context = DbContextHelper.CreateInMemoryDbContext("WarCryTests");
        _service = new WarCryService(_context, null);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoWarCries()
    {
        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnWarCries_WhenExists()
    {
        await SeedTestData();

        var result = await _service.GetAllAsync();

        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterBySearchTerm_WhenSearchProvided()
    {
        await SeedTestData();

        var result = await _service.GetAllAsync("transformação");

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("transformação");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnWarCry_WhenExists()
    {
        await SeedTestData();

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Title.Should().Be("A necessidade de transformação");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        await SeedTestData();

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingWarCries()
    {
        await SeedTestData();

        var result = await _service.SearchAsync("Jacó");

        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        await SeedTestData();

        var count = await _service.GetTotalCountAsync();

        count.Should().Be(2);
    }

    [Fact]
    public async Task ExistsByHashAsync_ShouldReturnTrue_WhenHashExists()
    {
        await SeedTestData();

        var exists = await _service.ExistsByHashAsync("abc123hash");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByHashAsync_ShouldReturnFalse_WhenHashNotExists()
    {
        await SeedTestData();

        var exists = await _service.ExistsByHashAsync("nonexistent");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveWarCry()
    {
        await SeedTestData();

        await _service.DeleteAsync(1);

        var result = await _service.GetByIdAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllEntitiesAsync_ShouldReturnEntities_WhenExists()
    {
        await SeedTestData();

        var result = await _service.GetAllEntitiesAsync();

        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        result.First().SyncStatus.Should().Be("active");
    }

    [Fact]
    public async Task UpdateContentAsync_ShouldUpdateContent_WhenWarCryExists()
    {
        await SeedTestData();

        await _service.UpdateContentAsync(1, "Novo conteúdo");

        var result = await _service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result!.Content.Should().Be("Novo conteúdo");
    }

    [Fact]
    public async Task SyncStatus_ShouldBeActiveByDefault_WhenCreated()
    {
        await SeedTestData();

        var result = await _service.GetAllEntitiesAsync();

        result.Should().AllSatisfy(wc => wc.SyncStatus.Should().Be("active"));
    }

    private async Task SeedTestData()
    {
        var warCries = new List<WarCry>
        {
            new WarCry
            {
                Id = 1,
                Title = "A necessidade de transformação",
                FileName = "Grito de Guerra_Msg 37_PDF_DIVULGAÇÃO_Jacó_A necessidade de transformação.pdf",
                Content = "Contenido do grito de guerra...",
                MessageNumber = 37,
                Theme = "Jacó",
                FileHash = "abc123hash",
                SourcePath = "/path/to/file.pdf",
                FileSize = 1024,
                SyncStatus = "active",
                SyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WarCry
            {
                Id = 2,
                Title = "Somos Vencedores",
                FileName = "Grito de Guerra_Msg 38_PDF_DIVULGAÇÃO_Pedro_Somos Vencedores.pdf",
                Content = "Outro contenido...",
                MessageNumber = 38,
                Theme = "Pedro",
                FileHash = "def456hash",
                SourcePath = "/path/to/file2.pdf",
                FileSize = 2048,
                SyncStatus = "active",
                SyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.WarCries.AddRange(warCries);
        await _context.SaveChangesAsync();
    }
}
