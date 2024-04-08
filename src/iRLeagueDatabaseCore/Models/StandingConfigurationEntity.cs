namespace iRLeagueDatabaseCore.Models;

public partial class StandingConfigurationEntity : IVersionEntity
{
    public StandingConfigurationEntity()
    {
        ChampSeasons = new HashSet<ChampSeasonEntity>();
        Standings = new HashSet<StandingEntity>();
    }

    public long LeagueId { get; set; }
    public long StandingConfigId { get; set; }

    public string Name { get; set; }
    public ResultKind ResultKind { get; set; }
    public bool UseCombinedResult { get; set; }
    public int WeeksCounted { get; set; }
    public ICollection<SortOptions> SortOptions { get; set; }

    public virtual LeagueEntity League { get; set; }
    public virtual IEnumerable<ChampSeasonEntity> ChampSeasons { get; set; }
    public virtual IEnumerable<StandingEntity> Standings { get; set; }

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

public sealed class StandingConfigurationEntityConfiguration : IEntityTypeConfiguration<StandingConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<StandingConfigurationEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.StandingConfigId });

        entity.HasAlternateKey(e => e.StandingConfigId);

        entity.Property(e => e.StandingConfigId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.SortOptions)
            .HasConversion(new CollectionToStringConverter<SortOptions>(), new ValueComparer<ICollection<SortOptions>>(true));

        entity.HasOne(d => d.League)
            .WithMany(p => p.StandingConfigs)
            .HasForeignKey(d => d.LeagueId);
    }
}
