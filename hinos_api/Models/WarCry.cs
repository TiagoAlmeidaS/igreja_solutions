using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hinos_api.Models;

[Table("war_cries")]
public class WarCry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int MessageNumber { get; set; }

    [MaxLength(200)]
    public string? Theme { get; set; }

    [MaxLength(2000)]
    public string SourcePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string FileHash { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime? FileModifiedAt { get; set; }

    public DateTime SyncedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(20)]
    public string SyncStatus { get; set; } = "active";
}
