namespace iRLeagueDatabaseCore.Models;

public partial class SessionEntity : IVersionEntity
{
    public SessionEntity()
    {
        IncidentReviews = new HashSet<IncidentReviewEntity>();
    }
    public long LeagueId { get; set; }
    public long SessionId { get; set; }
    public long EventId { get; set; }
    /// <summary>
    /// Number that decides order of subsessions
    /// </summary>
    public int SessionNr { get; set; }
    public string Name { get; set; }
    public SessionType SessionType { get; set; }
    public TimeSpan StartOffset { get; set; }
    public TimeSpan Duration { get; set; }
    public int Laps { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    #region version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion

    public virtual EventEntity Event { get; set; }
    public virtual SessionResultEntity SessionResult { get; set; }
    public virtual ICollection<IncidentReviewEntity> IncidentReviews { get; set; }
}

public class SubSessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.SessionId });

        entity.HasAlternateKey(e => e.SessionId);

        entity.Property(e => e.SessionId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => new { e.EventId, e.SessionId });

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.Property(e => e.StartOffset).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.Duration).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.SessionType)
            .HasConversion<string>();

        entity.HasOne(d => d.Event)
            .WithMany(p => p.Sessions)
            .HasForeignKey(d => new { d.LeagueId, d.EventId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
