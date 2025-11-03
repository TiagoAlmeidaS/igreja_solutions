namespace hinos_api.DTOs;

public class VerseDto
{
    public string Type { get; set; } = string.Empty;
    public List<string> Lines { get; set; } = new();
}

public class VerseInputDto
{
    public string Type { get; set; } = string.Empty;
    public List<string> Lines { get; set; } = new();
}
