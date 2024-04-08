namespace iRLeagueDatabaseCore.Models;

public class ScoredEventResultEntity : IVersionEntity
{
    public ScoredEventResultEntity()
    {
        ScoredSessionResults = new HashSet<ScoredSessionResultEntity>();
    }

    public long LeagueId { get; set; }
    public long ResultId { get; set; }
    public long EventId { get; set; }
    public long? ChampSeasonId { get; set; }
    public long? ResultConfigId { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public string Name { get; set; }

    public virtual EventEntity Event { get; set; }
    public virtual ChampSeasonEntity ChampSeason { get; set; }
    public virtual ICollection<ScoredSessionResultEntity> ScoredSessionResults { get; set; }
    public virtual ResultConfigurationEntity ResultConfig { get; set; }

    #region version
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    #endregion

}

public class ScoredEventResultEntityConfiguration : IEntityTypeConfiguration<ScoredEventResultEntity>
{
    public void Configure(EntityTypeBuilder<ScoredEventResultEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ResultId });

        entity.HasAlternateKey(e => new { e.ResultId });

        entity.Property(e => e.ResultId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.HasOne(d => d.Event)
            .WithMany(p => p.ScoredEventResults)
            .HasForeignKey(d => new { d.LeagueId, d.EventId })
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.ResultConfig)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.ResultConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
