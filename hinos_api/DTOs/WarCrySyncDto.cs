namespace hinos_api.DTOs;

public class WarCrySyncStatusDto
{
    public bool IsRunning { get; set; }
    public DateTime? LastSyncStart { get; set; }
    public DateTime? LastSyncEnd { get; set; }
    public int TotalWarCries { get; set; }
    public int NewWarCries { get; set; }
    public int UpdatedWarCries { get; set; }
    public int FailedCount { get; set; }
    public string? LastError { get; set; }
    public List<string> RecentFiles { get; set; } = new();
}

public class WarCrySyncResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalProcessed { get; set; }
    public int NewCount { get; set; }
    public int UpdatedCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime SyncStartTime { get; set; }
    public DateTime SyncEndTime { get; set; }
}
