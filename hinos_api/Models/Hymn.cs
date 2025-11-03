namespace hinos_api.Models;

public class Hymn
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // hinario, canticos, suplementar, novos
    public string HymnBook { get; set; } = string.Empty;
    public string? Key { get; set; }
    public int? Bpm { get; set; }
    public List<Verse> Verses { get; set; } = new();
}
