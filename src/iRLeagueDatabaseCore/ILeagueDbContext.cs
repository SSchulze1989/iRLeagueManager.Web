namespace iRLeagueDatabaseCore.Models;

public interface ILeagueDbContext
{
    public DbSet<AcceptedReviewVoteEntity> AcceptedReviewVotes { get; set; }
    public DbSet<AddPenaltyEntity> AddPenaltys { get; set; }
    public DbSet<AutoPenaltyConfigEntity> AutoPenaltyConfigs { get; set; }
    public DbSet<ReviewCommentEntity> ReviewComments { get; set; }
    public DbSet<ReviewCommentVoteEntity> ReviewCommentVotes { get; set; }
    public DbSet<CustomIncidentEntity> CustomIncidents { get; set; }
    public DbSet<ChampionshipEntity> Championships { get; set; }
    public DbSet<ChampSeasonEntity> ChampSeasons { get; set; }
    public DbSet<DriverStatisticRowEntity> DriverStatisticRows { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<IncidentReviewEntity> IncidentReviews { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }
    public DbSet<MemberEntity> Members { get; set; }
    public DbSet<LeagueMemberEntity> LeagueMembers { get; set; }
    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<PointRuleEntity> PointRules { get; set; }
    public DbSet<ProtestEntity> Protests { get; set; }
    public DbSet<EventResultEntity> EventResults { get; set; }
    public DbSet<ResultConfigurationEntity> ResultConfigurations { get; set; }
    public DbSet<ResultRowEntity> ResultRows { get; set; }
    public DbSet<FilterOptionEntity> FilterOptions { get; set; }
    public DbSet<ReviewPenaltyEntity> ReviewPenaltys { get; set; }
    public DbSet<ScheduleEntity> Schedules { get; set; }
    public DbSet<ScoredEventResultEntity> ScoredEventResults { get; set; }
    public DbSet<ScoredResultRowEntity> ScoredResultRows { get; set; }
    public DbSet<ScoredSessionResultEntity> ScoredSessionResults { get; set; }
    public DbSet<ScoringEntity> Scorings { get; set; }
    public DbSet<StandingEntity> Standings { get; set; }
    public DbSet<SeasonEntity> Seasons { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<StatisticSetEntity> StatisticSets { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    public DbSet<TeamEntity> Teams { get; set; }
    public DbSet<VoteCategoryEntity> VoteCategories { get; set; }
    public DbSet<TrackGroupEntity> TrackGroups { get; set; }
    public DbSet<IRSimSessionDetailsEntity> IRSimSessionDetails { get; set; }
    public DbSet<TrackConfigEntity> TrackConfigs { get; set; }
    public DbSet<SessionResultEntity> SessionResults { get; set; }
}