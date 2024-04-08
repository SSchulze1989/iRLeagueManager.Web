namespace iRLeagueDatabaseCore.Models;

public class EventEntity : IVersionEntity
{
    public EventEntity()
    {
        Sessions = new HashSet<SessionEntity>();
        ScoredEventResults = new HashSet<ScoredEventResultEntity>();
        ResultConfigs = new HashSet<ResultConfigurationEntity>();
        SimSessionDetails = new HashSet<IRSimSessionDetailsEntity>();
    }

    public long EventId { get; set; }
    public long LeagueId { get; set; }
    public long ScheduleId { get; set; }
    public long? TrackId { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public EventType EventType { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string Name { get; set; }
    public string IrSessionId { get; set; }
    public string IrResultLink { get; set; }

    public virtual ScheduleEntity Schedule { get; set; }
    public virtual TrackConfigEntity Track { get; set; }
    public virtual EventResultEntity EventResult { get; set; }
    public virtual ICollection<SessionEntity> Sessions { get; set; }
    public virtual ICollection<ScoredEventResultEntity> ScoredEventResults { get; set; }
    public virtual ICollection<ResultConfigurationEntity> ResultConfigs { get; set; }
    public virtual ICollection<IRSimSessionDetailsEntity> SimSessionDetails { get; set; }

    #region Version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion
}

public class EventEntityConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.EventId });

        entity.HasAlternateKey(e => e.EventId);

        entity.Property(e => e.Date).HasColumnType("datetime");

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.Property(e => e.Duration).HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.EventType)
            .HasConversion<string>();

        entity.Property(e => e.EventId)
            .ValueGeneratedOnAdd();

        entity.HasOne(e => e.Schedule)
            .WithMany(p => p.Events)
            .HasForeignKey(e => new { e.LeagueId, e.ScheduleId });

        entity.HasOne(d => d.Track)
            .WithMany()
            .HasForeignKey(d => d.TrackId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        entity.HasMany(d => d.ResultConfigs)
            .WithMany(p => p.Events)
            .UsingEntity<EventResultConfigs>(
                right => right.HasOne(e => e.ResultConfig)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.ResultConfigRefId }),
                left => left.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.EventRefId }));
    }
}
