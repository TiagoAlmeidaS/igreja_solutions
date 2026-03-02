namespace hinos_api.DTOs;

public class WarCryResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int MessageNumber { get; set; }
    public string? Theme { get; set; }
    public string SourcePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime? FileModifiedAt { get; set; }
    public DateTime SyncedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class WarCryListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MessageNumber { get; set; }
    public string? Theme { get; set; }
    public string ContentPreview { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; }
}
