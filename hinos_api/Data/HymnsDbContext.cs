using Microsoft.EntityFrameworkCore;
using hinos_api.Models;

namespace hinos_api.Data;

public class HymnsDbContext : DbContext
{
    public HymnsDbContext(DbContextOptions<HymnsDbContext> options) : base(options)
    {
    }

    public DbSet<Hymn> Hymns { get; set; }
    public DbSet<Verse> Verses { get; set; }
    public DbSet<WarCry> WarCries { get; set; }
    public DbSet<SyncLogEntry> SyncLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Hymn>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.HymnBook).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Key).HasMaxLength(10);
            
            entity.HasIndex(e => e.Number);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Title);
        });

        modelBuilder.Entity<Verse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.LinesJson).IsRequired();
            
            entity.HasOne(e => e.Hymn)
                .WithMany(h => h.Verses)
                .HasForeignKey(e => e.HymnId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WarCry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Theme).HasMaxLength(200);
            entity.Property(e => e.SourcePath).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.FileHash).IsRequired().HasMaxLength(64);
            entity.Property(e => e.SyncStatus).HasMaxLength(20).HasDefaultValue("active");
            
            entity.HasIndex(e => e.MessageNumber);
            entity.HasIndex(e => e.FileHash);
            entity.HasIndex(e => e.SyncedAt);
            entity.HasIndex(e => e.FileName);
            entity.HasIndex(e => e.SyncStatus);
        });
    }
}
