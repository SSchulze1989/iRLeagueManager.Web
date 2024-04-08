#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class EventResultEntity : IVersionEntity
{
    public EventResultEntity()
    {
        SessionResults = new HashSet<SessionResultEntity>();
        ScoredResults = new HashSet<ScoredEventResultEntity>();
    }

    public long LeagueId { get; set; }
    public long EventId { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    public bool RequiresRecalculation { get; set; }

    public virtual EventEntity Event { get; set; }
    public virtual ICollection<SessionResultEntity> SessionResults { get; set; }
    public virtual ICollection<ScoredEventResultEntity> ScoredResults { get; set; }
}

public class EventResultEntityConfiguration : IEntityTypeConfiguration<EventResultEntity>
{
    public void Configure(EntityTypeBuilder<EventResultEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.EventId });

        entity.HasIndex(e => e.EventId);

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.HasOne(d => d.Event)
            .WithOne(p => p.EventResult)
            .HasForeignKey<EventResultEntity>(d => new { d.LeagueId, d.EventId });
    }
}
