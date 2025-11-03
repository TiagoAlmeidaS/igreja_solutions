using FluentAssertions;
using hinos_api.DTOs;
using hinos_api.Models;
using hinos_api.Services;
using hinos_api.Tests.Helpers;

namespace hinos_api.Tests.Services;

public class HymnServiceTests
{
    [Fact]
    public void MapToDto_ShouldMapHymnToResponseDto_WhenHymnIsValid()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 1;
        hymn.Number = "101";
        hymn.Title = "Test Hymn";
        hymn.Category = "hinario";
        hymn.HymnBook = "Test Book";
        hymn.Key = "G";
        hymn.Bpm = 72;

        // Act
        var result = HymnService.MapToDto(hymn);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hymn.Id);
        result.Number.Should().Be(hymn.Number);
        result.Title.Should().Be(hymn.Title);
        result.Category.Should().Be(hymn.Category);
        result.HymnBook.Should().Be(hymn.HymnBook);
        result.Key.Should().Be(hymn.Key);
        result.Bpm.Should().Be(hymn.Bpm);
        result.Verses.Should().HaveCount(hymn.Verses.Count);
    }

    [Fact]
    public void MapToDto_ShouldMapVersesCorrectly_WhenHymnHasVerses()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Verses = HymnFaker.CreateFakeVerses(3);

        // Act
        var result = HymnService.MapToDto(hymn);

        // Assert
        result.Verses.Should().HaveCount(3);
        result.Verses[0].Type.Should().Be(hymn.Verses[0].Type);
        result.Verses[0].Lines.Should().BeEquivalentTo(hymn.Verses[0].Lines);
    }

    [Fact]
    public void MapToDto_ShouldMapOptionalFields_WhenKeyAndBpmAreNull()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Key = null;
        hymn.Bpm = null;

        // Act
        var result = HymnService.MapToDto(hymn);

        // Assert
        result.Key.Should().BeNull();
        result.Bpm.Should().BeNull();
    }

    [Fact]
    public void MapFromCreateDto_ShouldMapDtoToHymn_WhenDtoIsValid()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();

        // Act
        var result = HymnService.MapFromCreateDto(dto);

        // Assert
        result.Should().NotBeNull();
        result.Number.Should().Be(dto.Number);
        result.Title.Should().Be(dto.Title);
        result.Category.Should().Be(dto.Category);
        result.HymnBook.Should().Be(dto.HymnBook);
        result.Key.Should().Be(dto.Key);
        result.Bpm.Should().Be(dto.Bpm);
        result.Verses.Should().HaveCount(dto.Verses.Count);
    }

    [Fact]
    public void MapFromCreateDto_ShouldMapVersesCorrectly_WhenDtoHasVerses()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();
        dto.Verses = new List<VerseInputDto>
        {
            new() { Type = "V1", Lines = new List<string> { "Line 1", "Line 2" } },
            new() { Type = "R", Lines = new List<string> { "Refrain 1", "Refrain 2" } }
        };

        // Act
        var result = HymnService.MapFromCreateDto(dto);

        // Assert
        result.Verses.Should().HaveCount(2);
        result.Verses[0].Type.Should().Be("V1");
        result.Verses[0].Lines.Should().BeEquivalentTo(new List<string> { "Line 1", "Line 2" });
        result.Verses[1].Type.Should().Be("R");
        result.Verses[1].Lines.Should().BeEquivalentTo(new List<string> { "Refrain 1", "Refrain 2" });
    }

    [Fact]
    public void MapFromCreateDto_ShouldSetIdToZero_WhenCreatingNewHymn()
    {
        // Arrange
        var dto = HymnFaker.CreateFakeCreateHymnDto();

        // Act
        var result = HymnService.MapFromCreateDto(dto);

        // Assert
        result.Id.Should().Be(0);
    }

    [Fact]
    public void UpdateFromDto_ShouldUpdateHymnProperties_WhenDtoIsValid()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 1;
        var originalNumber = hymn.Number;
        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Number = "999";

        // Act
        HymnService.UpdateFromDto(hymn, dto);

        // Assert
        hymn.Number.Should().Be(dto.Number).And.NotBe(originalNumber);
        hymn.Title.Should().Be(dto.Title);
        hymn.Category.Should().Be(dto.Category);
        hymn.HymnBook.Should().Be(dto.HymnBook);
        hymn.Key.Should().Be(dto.Key);
        hymn.Bpm.Should().Be(dto.Bpm);
    }

    [Fact]
    public void UpdateFromDto_ShouldClearAndReplaceVerses_WhenDtoHasVerses()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 1;
        hymn.Verses = HymnFaker.CreateFakeVerses(3);
        var originalVerseCount = hymn.Verses.Count;

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Verses = new List<VerseInputDto>
        {
            new() { Type = "V1", Lines = new List<string> { "New Line 1" } }
        };

        // Act
        HymnService.UpdateFromDto(hymn, dto);

        // Assert
        hymn.Verses.Should().HaveCount(1).And.NotHaveCount(originalVerseCount);
        hymn.Verses[0].Type.Should().Be("V1");
        hymn.Verses[0].Lines.Should().BeEquivalentTo(new List<string> { "New Line 1" });
        hymn.Verses[0].HymnId.Should().Be(hymn.Id);
    }

    [Fact]
    public void UpdateFromDto_ShouldUpdateOptionalFieldsToNull_WhenDtoHasNullValues()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 1;
        hymn.Key = "G";
        hymn.Bpm = 72;

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Key = null;
        dto.Bpm = null;

        // Act
        HymnService.UpdateFromDto(hymn, dto);

        // Assert
        hymn.Key.Should().BeNull();
        hymn.Bpm.Should().BeNull();
    }

    [Fact]
    public void UpdateFromDto_ShouldHandleEmptyVersesList_WhenDtoHasNoVerses()
    {
        // Arrange
        var hymn = HymnFaker.CreateFakeHymn();
        hymn.Id = 1;
        hymn.Verses = HymnFaker.CreateFakeVerses(2);

        var dto = HymnFaker.CreateFakeUpdateHymnDto();
        dto.Verses = new List<VerseInputDto>();

        // Act
        HymnService.UpdateFromDto(hymn, dto);

        // Assert
        hymn.Verses.Should().BeEmpty();
    }
}
