namespace iRLeagueDatabaseCore.Models;

public class ResultConfigurationEntity : IVersionEntity
{
    public ResultConfigurationEntity()
    {
        Scorings = new HashSet<ScoringEntity>();
        Events = new HashSet<EventEntity>();
        PointFilters = new HashSet<FilterOptionEntity>();
        ResultFilters = new HashSet<FilterOptionEntity>();
    }

    public long LeagueId { get; set; }
    public long ResultConfigId { get; set; }
    public long ChampSeasonId { get; set; }
    public long? SourceResultConfigId { get; set; }

    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int ResultsPerTeam { get; set; }

    public virtual LeagueEntity League { get; set; }
    public virtual ResultConfigurationEntity SourceResultConfig { get; set; }
    public virtual ChampSeasonEntity ChampSeason { get; set; }
    public virtual ICollection<ScoringEntity> Scorings { get; set; }
    public virtual IEnumerable<EventEntity> Events { get; set; }
    public virtual ICollection<FilterOptionEntity> PointFilters { get; set; }
    public virtual ICollection<FilterOptionEntity> ResultFilters { get; set; }
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

public sealed class ResultConfigurationEntityConfiguration : IEntityTypeConfiguration<ResultConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<ResultConfigurationEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ResultConfigId });

        entity.HasAlternateKey(e => e.ResultConfigId);

        entity.Property(e => e.ResultConfigId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.HasOne(d => d.League)
            .WithMany(p => p.ResultConfigs)
            .HasForeignKey(d => d.LeagueId);

        entity.HasOne(d => d.SourceResultConfig)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.SourceResultConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
