using hinos_api.Models;
using hinos_api.DTOs;
using Microsoft.EntityFrameworkCore;
using hinos_api.Data;

namespace hinos_api.Services;

public static class HymnService
{
    public static HymnResponseDto MapToDto(Hymn hymn)
    {
        return new HymnResponseDto
        {
            Id = hymn.Id,
            Number = hymn.Number,
            Title = hymn.Title,
            Category = hymn.Category,
            HymnBook = hymn.HymnBook,
            Key = hymn.Key,
            Bpm = hymn.Bpm,
            Verses = hymn.Verses.Select(v => new VerseDto
            {
                Type = v.Type,
                Lines = v.Lines
            }).ToList()
        };
    }

    public static Hymn MapFromCreateDto(CreateHymnDto dto)
    {
        var hymn = new Hymn
        {
            Number = dto.Number,
            Title = dto.Title,
            Category = dto.Category,
            HymnBook = dto.HymnBook,
            Key = dto.Key,
            Bpm = dto.Bpm
        };

        hymn.Verses = dto.Verses.Select(v => new Verse
        {
            Type = v.Type,
            Lines = v.Lines
        }).ToList();

        return hymn;
    }

    public static void UpdateFromDto(Hymn hymn, UpdateHymnDto dto)
    {
        hymn.Number = dto.Number;
        hymn.Title = dto.Title;
        hymn.Category = dto.Category;
        hymn.HymnBook = dto.HymnBook;
        hymn.Key = dto.Key;
        hymn.Bpm = dto.Bpm;

        // Remove versos existentes
        hymn.Verses.Clear();
        
        // Adiciona novos versos
        foreach (var verseDto in dto.Verses)
        {
            hymn.Verses.Add(new Verse
            {
                Type = verseDto.Type,
                Lines = verseDto.Lines,
                HymnId = hymn.Id
            });
        }
    }
}
