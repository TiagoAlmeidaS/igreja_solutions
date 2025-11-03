namespace hinos_api.DTOs;

public class UpdateHymnDto
{
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string HymnBook { get; set; } = string.Empty;
    public string? Key { get; set; }
    public int? Bpm { get; set; }
    public List<VerseInputDto> Verses { get; set; } = new();
}
