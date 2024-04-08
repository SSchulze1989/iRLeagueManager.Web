namespace iRLeagueDatabaseCore.Models;

public class LeagueEntity : Revision, IVersionEntity
{
    public LeagueEntity()
    {
        Seasons = new HashSet<SeasonEntity>();
        ResultConfigs = new HashSet<ResultConfigurationEntity>();
        PointRules = new HashSet<PointRuleEntity>();
        LeagueMembers = new HashSet<LeagueMemberEntity>();
        Teams = new HashSet<TeamEntity>();
        VoteCategories = new HashSet<VoteCategoryEntity>();
        Championships = new HashSet<ChampionshipEntity>();
        StandingConfigs = new HashSet<StandingConfigurationEntity>();
    }
    public long Id { get; set; }
    public string Name { get; set; }
    public string NameFull { get; set; }
    public string Description { get; set; }
    public string DescriptionPlain { get; set; }
    public bool IsInitialized { get; set; }
    public bool EnableProtests { get; set; }
    /// <summary>
    /// Time span after a race has finished (according to event duration) after which a protest can be filed
    /// </summary>
    public TimeSpan ProtestCoolDownPeriod { get; set; }
    /// <summary>
    /// Time span after a race has finished (according to event duration) until a protest can be filed
    /// </summary>
    public TimeSpan ProtestsClosedAfter { get; set; }
    /// <summary>
    /// Set public visibility of protests
    /// </summary>
    public ProtestPublicSetting ProtestsPublic { get; set; }
    public ProtestFormAccess ProtestFormAccess { get; set; }
    public LeaguePublicSetting LeaguePublic { get; set; }
    public bool EnableLiveReviews { get; set; }
    public SubscriptionStatus Subscription { get; set; }
    public DateTime? Expires { get; set; }

    public virtual ICollection<SeasonEntity> Seasons { get; set; }
    public virtual ICollection<ResultConfigurationEntity> ResultConfigs { get; set; }
    public virtual ICollection<PointRuleEntity> PointRules { get; set; }
    public virtual IEnumerable<ScoringEntity> Scorings { get; set; }
    public virtual ICollection<LeagueMemberEntity> LeagueMembers { get; set; }
    public virtual ICollection<TeamEntity> Teams { get; set; }
    public virtual ICollection<VoteCategoryEntity> VoteCategories { get; set; }
    public virtual ICollection<ChampionshipEntity> Championships { get; set; }
    public virtual ICollection<StandingConfigurationEntity> StandingConfigs { get; set; }
    public virtual IEnumerable<PaymentEntity> Payments { get; set; }
}

public class LeagueEntityConfiguration : IEntityTypeConfiguration<LeagueEntity>
{
    public void Configure(EntityTypeBuilder<LeagueEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.HasAlternateKey(e => e.Name);

        entity.Property(e => e.Name)
            .HasMaxLength(85);

        entity.Property(e => e.ProtestCoolDownPeriod)
            .HasConversion<TimeSpanToTicksConverter>();

        entity.Property(e => e.ProtestsClosedAfter)
            .HasConversion<TimeSpanToTicksConverter>();

        entity.HasMany(d => d.Scorings)
            .WithOne()
            .HasForeignKey(p => p.LeagueId);
    }
}
