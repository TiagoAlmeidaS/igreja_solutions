namespace hinos_api.DTOs;

public class HymnResponseDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string HymnBook { get; set; } = string.Empty;
    public string? Key { get; set; }
    public int? Bpm { get; set; }
    public List<VerseDto> Verses { get; set; } = new();
}
