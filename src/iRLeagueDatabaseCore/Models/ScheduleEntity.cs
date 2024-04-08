#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class ScheduleEntity : IVersionEntity
{
    public ScheduleEntity()
    {
        Scorings = new HashSet<ScoringEntity>();
        Events = new HashSet<EventEntity>();
    }

    public long ScheduleId { get; set; }
    public long LeagueId { get; set; }
    public string Name { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public long SeasonId { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public virtual SeasonEntity Season { get; set; }
    public virtual ICollection<ScoringEntity> Scorings { get; set; }
    public virtual ICollection<EventEntity> Events { get; set; }
}

public class ScheduleEntityConfiguration : IEntityTypeConfiguration<ScheduleEntity>
{
    public void Configure(EntityTypeBuilder<ScheduleEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ScheduleId });

        entity.HasAlternateKey(e => e.ScheduleId);

        entity.Property(e => e.ScheduleId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => new { e.LeagueId, e.SeasonId });

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.HasOne(d => d.Season)
            .WithMany(p => p.Schedules)
            .HasForeignKey(d => new { d.LeagueId, d.SeasonId });
    }
}
