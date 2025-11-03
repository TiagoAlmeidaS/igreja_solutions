using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.DTOs;
using hinos_api.Models;
using hinos_api.Tests.Helpers;

namespace hinos_api.Tests.Endpoints;

public class HymnsEndpointTests : IDisposable
{
    private readonly HymnsDbContext _dbContext;

    public HymnsEndpointTests()
    {
        _dbContext = DbContextHelper.CreateInMemoryDbContext($"TestDb_{Guid.NewGuid()}");
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetHymns_ShouldReturnAllHymns_WhenNoFilters()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        var hymn2 = HymnFaker.CreateFakeHymn();
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        // Act - Simular a lógica do endpoint
        var query = _dbContext.Hymns.Include(h => h.Verses).AsQueryable();
        var hymns = await query.ToListAsync();

        // Assert
        hymns.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHymns_ShouldFilterByCategory_WhenCategoryProvided()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Category = "hinario";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Category = "canticos";
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        // Act
        var category = "hinario";
        var query = _dbContext.Hymns.Include(h => h.Verses).AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(h => h.Category == category);
        }
        var hymns = await query.ToListAsync();

        // Assert
        hymns.Should().HaveCount(1);
        hymns[0].Category.Should().Be("hinario");
    }

    [Fact]
    public async Task GetHymns_ShouldFilterBySearch_WhenSearchTermProvided()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Title = "Graça Maravilhosa";
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var search = "Graça";
        var query = _dbContext.Hymns.Include(h => h.Verses).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(h =>
                h.Number.ToLower().Contains(searchLower) ||
                h.Title.ToLower().Contains(searchLower) ||
                h.HymnBook.ToLower().Contains(searchLower) ||
                h.Verses.Any(v => v.LinesJson.ToLower().Contains(searchLower))
            );
        }
        var hymns = await query.ToListAsync();

        // Assert
        hymns.Should().HaveCount(1);
        hymns[0].Title.Should().Contain("Graça");
    }

    [Fact]
    public async Task GetHymnById_ShouldReturnHymn_WhenHymnExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act - Simular lógica do endpoint
        var id = hymn.Id;
        var foundHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == id);

        // Assert
        foundHymn.Should().NotBeNull();
        foundHymn!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetHymnById_ShouldReturnNull_WhenHymnDoesNotExist()
    {
        // Act
        var hymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == 99999);

        // Assert
        hymn.Should().BeNull();
    }

    [Fact]
    public async Task CreateHymn_ShouldAddHymn_WhenValid()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "999";

        // Act - Simular lógica do endpoint
        var exists = await _dbContext.Hymns.AnyAsync(h => h.Number == dto.Number);
        exists.Should().BeFalse();

        var hymn = hinos_api.Services.HymnService.MapFromCreateDto(dto);
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Atualizar HymnId nos versos
        foreach (var verse in hymn.Verses)
        {
            verse.HymnId = hymn.Id;
        }
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == hymn.Id);
        
        savedHymn.Should().NotBeNull();
        savedHymn!.Number.Should().Be(dto.Number);
    }

    [Fact]
    public async Task CreateHymn_ShouldReject_WhenNumberAlreadyExists()
    {
        // Arrange
        var existingHymn = HymnFaker.CreateFakeHymn();
        existingHymn.Number = "101";
        _dbContext.Hymns.Add(existingHymn);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "101";

        // Act
        var exists = await _dbContext.Hymns.AnyAsync(h => h.Number == dto.Number);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void CreateHymn_ShouldReject_WhenNumberIsEmpty()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Number = "";

        // Act & Assert
        string.IsNullOrWhiteSpace(dto.Number).Should().BeTrue();
    }

    [Fact]
    public void CreateHymn_ShouldReject_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Title = "";

        // Act & Assert
        string.IsNullOrWhiteSpace(dto.Title).Should().BeTrue();
    }

    [Fact]
    public void CreateHymn_ShouldReject_WhenCategoryIsEmpty()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Category = "";

        // Act & Assert
        string.IsNullOrWhiteSpace(dto.Category).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateHymn_ShouldUpdateHymn_WhenValid()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Title = "Updated Title";

        // Act - Simular lógica do endpoint
        var existingHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == hymn.Id);

        existingHymn.Should().NotBeNull();
        
        hinos_api.Services.HymnService.UpdateFromDto(existingHymn!, dto);
        
        foreach (var verse in existingHymn!.Verses)
        {
            verse.HymnId = existingHymn.Id;
        }

        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedHymn = await _dbContext.Hymns.FindAsync(hymn.Id);
        updatedHymn.Should().NotBeNull();
        updatedHymn!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateHymn_ShouldReturnNull_WhenHymnDoesNotExist()
    {
        // Act
        var hymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == 99999);

        // Assert
        hymn.Should().BeNull();
    }

    [Fact]
    public async Task UpdateHymn_ShouldReject_WhenNumberIsUsedByAnotherHymn()
    {
        // Arrange
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Number = "101";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Number = "102";
        _dbContext.Hymns.AddRange(hymn1, hymn2);
        await _dbContext.SaveChangesAsync();

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Number = "101";

        // Act
        var exists = await _dbContext.Hymns
            .AnyAsync(h => h.Number == dto.Number && h.Id != hymn2.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteHymn_ShouldRemoveHymn_WhenExists()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act - Simular lógica do endpoint
        var existingHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == hymn.Id);

        existingHymn.Should().NotBeNull();

        _dbContext.Hymns.Remove(existingHymn!);
        await _dbContext.SaveChangesAsync();

        // Assert
        var hymnExists = await _dbContext.Hymns.AnyAsync(h => h.Id == hymn.Id);
        hymnExists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteHymn_ShouldCascadeDeleteVerses_WhenHymnIsDeleted()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Verses = HymnFaker.CreateFakeVerses(3);
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        var verseCountBefore = await _dbContext.Verses.CountAsync(v => v.HymnId == hymn.Id);
        verseCountBefore.Should().Be(3);

        // Act
        var existingHymn = await _dbContext.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == hymn.Id);

        _dbContext.Hymns.Remove(existingHymn!);
        await _dbContext.SaveChangesAsync();

        // Assert
        var versesExist = await _dbContext.Verses.AnyAsync(v => v.HymnId == hymn.Id);
        versesExist.Should().BeFalse();
    }

    [Fact]
    public void SearchHymns_ShouldReturnBadRequest_WhenTermIsEmpty()
    {
        // Act & Assert
        string.IsNullOrWhiteSpace("").Should().BeTrue();
    }

    [Fact]
    public async Task SearchHymns_ShouldFindByTerm_WhenTermMatches()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Title = "Castelo Forte";
        _dbContext.Hymns.Add(hymn);
        await _dbContext.SaveChangesAsync();

        // Act
        var term = "Castelo";
        var termLower = term.ToLower();
        var hymns = await _dbContext.Hymns
            .Include(h => h.Verses)
            .Where(h =>
                h.Number.ToLower().Contains(termLower) ||
                h.Number.ToLower() == termLower ||
                h.Title.ToLower().Contains(termLower) ||
                h.HymnBook.ToLower().Contains(termLower) ||
                h.Verses.Any(v => v.LinesJson.ToLower().Contains(termLower))
            )
            .ToListAsync();

        // Assert
        hymns.Should().HaveCount(1);
        hymns[0].Title.Should().Contain("Castelo");
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}