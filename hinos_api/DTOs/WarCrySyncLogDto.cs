namespace hinos_api.DTOs;

public class WarCrySyncLogDto
{
    public int Id { get; set; }
    public DateTime SyncStartTime { get; set; }
    public DateTime? SyncEndTime { get; set; }
    public string SyncType { get; set; } = "incremental";
    public int TotalFilesFound { get; set; }
    public int NewCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int FailedCount { get; set; }
    public int OrphanCount { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ProcessedFileNames { get; set; } = new();
    public List<string> FailedFileNames { get; set; } = new();
}

public class WarCrySyncRequestDto
{
    public bool FullSync { get; set; } = false;
}
