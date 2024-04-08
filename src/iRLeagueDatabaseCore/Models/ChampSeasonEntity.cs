using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueDatabaseCore.Models;
public partial class ChampSeasonEntity : IVersionEntity
{
    public ChampSeasonEntity()
    {
        ResultConfigurations = new HashSet<ResultConfigurationEntity>();
        EventResults = new HashSet<ScoredEventResultEntity>();
        Standings = new HashSet<StandingEntity>();
        Filters = new HashSet<FilterOptionEntity>();
    }

    public long LeagueId { get; set; }
    public long ChampSeasonId { get; set; }
    public long ChampionshipId { get; set; }
    public long SeasonId { get; set; }
    public long? DefaultResultConfigId { get; set; }
    public long? StandingConfigId { get; set; }
    public ResultKind ResultKind { get; set; }
    public bool IsActive { get; set; }

    public virtual ChampionshipEntity Championship { get; set; }
    public virtual SeasonEntity Season { get; set; }
    public virtual ResultConfigurationEntity DefaultResultConfig { get; set; }
    public virtual ICollection<ResultConfigurationEntity> ResultConfigurations { get; set; }
    public virtual StandingConfigurationEntity StandingConfiguration { get; set; }
    public virtual IEnumerable<ScoredEventResultEntity> EventResults { get; set; }
    public virtual IEnumerable<StandingEntity> Standings { get; set; }
    public virtual ICollection<FilterOptionEntity> Filters { get; set; }

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

public sealed class ChampSeasonEntityConfiguration : IEntityTypeConfiguration<ChampSeasonEntity>
{
    public void Configure(EntityTypeBuilder<ChampSeasonEntity> entity)
    {
        entity.ToTable("ChampSeasons");

        entity.HasKey(e => new { e.LeagueId, e.ChampSeasonId });

        entity.HasAlternateKey(e => e.ChampSeasonId);

        entity.Property(e => e.ChampSeasonId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.ResultKind)
            .HasConversion<EnumToStringConverter<ResultKind>>()
            .HasMaxLength(50);

        entity.HasOne(d => d.Championship)
            .WithMany(p => p.ChampSeasons)
            .HasForeignKey(d => new { d.LeagueId, d.ChampionshipId })
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.Season)
            .WithMany(p => p.ChampSeasons)
            .HasForeignKey(d => new { d.LeagueId, d.SeasonId })
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.StandingConfiguration)
            .WithMany(p => p.ChampSeasons)
            .HasForeignKey(d => new { d.LeagueId, d.StandingConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasMany(d => d.ResultConfigurations)
            .WithOne(p => p.ChampSeason)
            .HasForeignKey(d => new { d.LeagueId, d.ChampSeasonId });

        entity.HasMany(p => p.EventResults)
            .WithOne(d => d.ChampSeason)
            .HasForeignKey(d => new { d.LeagueId, d.ChampSeasonId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasOne(d => d.DefaultResultConfig)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.DefaultResultConfigId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        entity.HasMany(p => p.Standings)
            .WithOne(d => d.ChampSeason)
            .HasForeignKey(d => new { d.LeagueId, d.ChampSeasonId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
