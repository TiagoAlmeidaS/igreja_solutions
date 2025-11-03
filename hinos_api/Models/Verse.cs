using System.Text.Json;

namespace hinos_api.Models;

public class Verse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // V1, V2, V3, V4, R, Ponte, C
    public string LinesJson { get; set; } = "[]"; // Armazena as linhas como JSON
    public int HymnId { get; set; }
    public Hymn? Hymn { get; set; }

    // Propriedade para acessar as linhas como lista
    public List<string> Lines
    {
        get => JsonSerializer.Deserialize<List<string>>(LinesJson) ?? new List<string>();
        set => LinesJson = JsonSerializer.Serialize(value);
    }
}
