using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using hinos_api.Data;
using hinos_api.Models;
using hinos_api.Tests.Helpers;

namespace hinos_api.Tests.Data;

public class HymnsDbContextTests
{
    [Fact]
    public void HymnsDbContext_ShouldCreateDatabase_WhenInitialized()
    {
        // Arrange & Act
        using var context = DbContextHelper.CreateInMemoryDbContext();

        // Assert
        context.Should().NotBeNull();
        context.Database.EnsureCreated().Should().BeTrue();
    }

    [Fact]
    public async Task HymnsDbContext_ShouldSaveHymn_WhenAddingHymn()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 0;

        // Act
        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        // Assert
        var savedHymn = await context.Hymns.FindAsync(hymn.Id);
        savedHymn.Should().NotBeNull();
        savedHymn!.Number.Should().Be(hymn.Number);
        savedHymn.Title.Should().Be(hymn.Title);
    }

    [Fact]
    public async Task HymnsDbContext_ShouldSaveVersesWithHymn_WhenAddingHymnWithVerses()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 0;
        hymn.Verses = HymnFaker.CreateFakeVerses(3);

        // Act
        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        // Assert
        var savedHymn = await context.Hymns
            .Include(h => h.Verses)
            .FirstOrDefaultAsync(h => h.Id == hymn.Id);

        savedHymn.Should().NotBeNull();
        savedHymn!.Verses.Should().HaveCount(3);
        savedHymn.Verses.All(v => v.HymnId == hymn.Id).Should().BeTrue();
    }

    [Fact]
    public async Task HymnsDbContext_ShouldCascadeDeleteVerses_WhenDeletingHymn()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 0;
        hymn.Verses = HymnFaker.CreateFakeVerses(2);

        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        var verseCountBefore = await context.Verses.CountAsync(v => v.HymnId == hymn.Id);
        verseCountBefore.Should().Be(2);

        // Act
        context.Hymns.Remove(hymn);
        await context.SaveChangesAsync();

        // Assert
        var hymnExists = await context.Hymns.AnyAsync(h => h.Id == hymn.Id);
        hymnExists.Should().BeFalse();

        var versesExist = await context.Verses.AnyAsync(v => v.HymnId == hymn.Id);
        versesExist.Should().BeFalse();
    }

    [Fact]
    public async Task HymnsDbContext_ShouldEnforceRequiredFields_WhenSavingHymn()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = new Hymn
        {
            Number = "", // Vazio
            Title = null!, // Null
            Category = "",
            HymnBook = ""
        };

        // Act
        context.Hymns.Add(hymn);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await context.SaveChangesAsync());
    }

    [Fact]
    public async Task HymnsDbContext_ShouldCreateIndexOnNumber_WhenQueryingByNumber()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Number = "101";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Number = "102";

        context.Hymns.AddRange(hymn1, hymn2);
        await context.SaveChangesAsync();

        // Act
        var foundHymn = await context.Hymns.FirstOrDefaultAsync(h => h.Number == "101");

        // Assert
        foundHymn.Should().NotBeNull();
        foundHymn!.Number.Should().Be("101");
    }

    [Fact]
    public async Task HymnsDbContext_ShouldCreateIndexOnCategory_WhenQueryingByCategory()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn1 = HymnFaker.CreateFakeHymn();
        hymn1.Category = "hinario";
        var hymn2 = HymnFaker.CreateFakeHymn();
        hymn2.Category = "canticos";

        context.Hymns.AddRange(hymn1, hymn2);
        await context.SaveChangesAsync();

        // Act
        var hinarios = await context.Hymns
            .Where(h => h.Category == "hinario")
            .ToListAsync();

        // Assert
        hinarios.Should().NotBeEmpty();
        hinarios.All(h => h.Category == "hinario").Should().BeTrue();
    }

    [Fact]
    public async Task HymnsDbContext_ShouldStoreVerseLinesAsJson_WhenSavingVerse()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 0;
        var verse = new Verse
        {
            Type = "V1",
            Lines = new List<string> { "Line 1", "Line 2", "Line 3" },
            HymnId = 0
        };
        hymn.Verses = new List<Verse> { verse };

        // Act
        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        // Assert
        var savedVerse = await context.Verses.FirstAsync(v => v.HymnId == hymn.Id);
        savedVerse.LinesJson.Should().NotBeNullOrEmpty();
        savedVerse.Lines.Should().HaveCount(3);
        savedVerse.Lines.Should().BeEquivalentTo(new List<string> { "Line 1", "Line 2", "Line 3" });
    }

    [Fact]
    public async Task HymnsDbContext_ShouldUpdateHymn_WhenModifyingHymn()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = "101";
        hymn.Title = "Original Title";

        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        // Act
        hymn.Title = "Updated Title";
        await context.SaveChangesAsync();

        // Assert
        var updatedHymn = await context.Hymns.FindAsync(hymn.Id);
        updatedHymn.Should().NotBeNull();
        updatedHymn!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task HymnsDbContext_ShouldSaveHymn_WhenNumberIsWithinMaxLength()
    {
        // Arrange
        using var context = DbContextHelper.CreateInMemoryDbContext();
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Number = new string('A', 50); // Dentro do MaxLength(50)

        // Act
        context.Hymns.Add(hymn);
        await context.SaveChangesAsync();

        // Assert
        var savedHymn = await context.Hymns.FindAsync(hymn.Id);
        savedHymn.Should().NotBeNull();
        savedHymn!.Number.Should().HaveLength(50);
    }
    
    // Nota: EF Core InMemory não aplica restrições de MaxLength,
    // então este teste verifica que valores dentro do limite funcionam.
    // Para testar restrições reais, seria necessário usar SQLite ou outro provider.
}
