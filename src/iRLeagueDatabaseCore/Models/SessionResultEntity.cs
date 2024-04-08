namespace iRLeagueDatabaseCore.Models;

public partial class SessionResultEntity : IVersionEntity
{
    public SessionResultEntity()
    {
        ResultRows = new HashSet<ResultRowEntity>();
    }

    public long LeagueId { get; set; }
    public long EventId { get; set; }
    public long SessionId { get; set; }
    public long? IRSimSessionDetailsId { get; set; }

    public SimSessionType SimSessionType { get; set; }

    #region version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion

    public virtual EventResultEntity Result { get; set; }
    public virtual SessionEntity Session { get; set; }
    public virtual ICollection<ResultRowEntity> ResultRows { get; set; }
    public virtual IRSimSessionDetailsEntity IRSimSessionDetails { get; set; }
}

public class SessionResultEntityConfiguration : IEntityTypeConfiguration<SessionResultEntity>
{
    public void Configure(EntityTypeBuilder<SessionResultEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.SessionId });

        entity.HasIndex(e => e.SessionId);

        entity.HasOne(d => d.Result)
            .WithMany(p => p.SessionResults)
            .HasForeignKey(d => new { d.LeagueId, d.EventId });

        entity.HasOne(d => d.Session)
            .WithOne(p => p.SessionResult)
            .HasForeignKey<SessionResultEntity>(d => new { d.LeagueId, d.SessionId })
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.IRSimSessionDetails)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.IRSimSessionDetailsId });
    }
}