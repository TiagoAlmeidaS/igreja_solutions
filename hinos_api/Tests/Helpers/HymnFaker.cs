using Bogus;
using hinos_api.DTOs;
using hinos_api.Models;

namespace hinos_api.Tests.Helpers;

public static class HymnFaker
{
    private static readonly Faker _faker = new("pt_BR");

    public static Hymn CreateFakeHymn(int? id = null)
    {
        var verseTypes = new[] { "V1", "V2", "V3", "V4", "R", "Ponte", "C" };
        var categories = new[] { "hinario", "canticos", "suplementar", "novos" };
        var keys = new[] { "C", "D", "E", "F", "G", "A", "B" };

        var hymn = new Faker<Hymn>("pt_BR")
            .RuleFor(h => h.Id, f => id ?? 0)
            .RuleFor(h => h.Number, f => f.Random.Number(1, 999).ToString())
            .RuleFor(h => h.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(h => h.Category, f => f.PickRandom(categories))
            .RuleFor(h => h.HymnBook, f => $"Hinário {f.Company.CompanyName()}")
            .RuleFor(h => h.Key, f => f.PickRandom(keys))
            .RuleFor(h => h.Bpm, f => f.Random.Int(60, 120))
            .RuleFor(h => h.Verses, f => CreateFakeVerses(f.Random.Int(2, 4)))
            .Generate();

        // Configurar HymnId nos versos
        foreach (var verse in hymn.Verses)
        {
            verse.HymnId = hymn.Id;
        }

        return hymn;
    }

    public static List<Verse> CreateFakeVerses(int count)
    {
        var verseTypes = new[] { "V1", "V2", "V3", "V4", "R", "Ponte", "C" };
        
        return new Faker<Verse>("pt_BR")
            .RuleFor(v => v.Id, f => 0)
            .RuleFor(v => v.Type, f => f.PickRandom(verseTypes))
            .RuleFor(v => v.Lines, f => f.Lorem.Sentences(f.Random.Int(2, 4)).Split('.').Take(4).ToList())
            .RuleFor(v => v.HymnId, f => 0)
            .Generate(count);
    }

    public static CreateHymnDto CreateFakeCreateHymnDto()
    {
        var verseTypes = new[] { "V1", "V2", "V3", "V4", "R", "Ponte", "C" };
        var categories = new[] { "hinario", "canticos", "suplementar", "novos" };
        var keys = new[] { "C", "D", "E", "F", "G", "A", "B" };

        return new Faker<CreateHymnDto>("pt_BR")
            .RuleFor(dto => dto.Number, f => f.Random.Number(1, 999).ToString())
            .RuleFor(dto => dto.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(dto => dto.Category, f => f.PickRandom(categories))
            .RuleFor(dto => dto.HymnBook, f => $"Hinário {f.Company.CompanyName()}")
            .RuleFor(dto => dto.Key, f => f.PickRandom(keys))
            .RuleFor(dto => dto.Bpm, f => f.Random.Int(60, 120))
            .RuleFor(dto => dto.Verses, f => CreateFakeVerseInputDtos(f.Random.Int(2, 4)))
            .Generate();
    }

    public static UpdateHymnDto CreateFakeUpdateHymnDto()
    {
        var verseTypes = new[] { "V1", "V2", "V3", "V4", "R", "Ponte", "C" };
        var categories = new[] { "hinario", "canticos", "suplementar", "novos" };
        var keys = new[] { "C", "D", "E", "F", "G", "A", "B" };

        return new Faker<UpdateHymnDto>("pt_BR")
            .RuleFor(dto => dto.Number, f => f.Random.Number(1, 999).ToString())
            .RuleFor(dto => dto.Title, f => f.Lorem.Sentence(3, 5))
            .RuleFor(dto => dto.Category, f => f.PickRandom(categories))
            .RuleFor(dto => dto.HymnBook, f => $"Hinário {f.Company.CompanyName()}")
            .RuleFor(dto => dto.Key, f => f.PickRandom(keys))
            .RuleFor(dto => dto.Bpm, f => f.Random.Int(60, 120))
            .RuleFor(dto => dto.Verses, f => CreateFakeVerseInputDtos(f.Random.Int(2, 4)))
            .Generate();
    }

    private static List<VerseInputDto> CreateFakeVerseInputDtos(int count)
    {
        var verseTypes = new[] { "V1", "V2", "V3", "V4", "R", "Ponte", "C" };

        return new Faker<VerseInputDto>("pt_BR")
            .RuleFor(v => v.Type, f => f.PickRandom(verseTypes))
            .RuleFor(v => v.Lines, f => f.Lorem.Sentences(f.Random.Int(2, 4)).Split('.').Take(4).ToList())
            .Generate(count);
    }
}
