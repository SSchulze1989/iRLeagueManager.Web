using iRLeagueApiCore.Common;
using iRLeagueDatabaseCore;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Mocking.DataAccess;

public abstract class DataAccessTestsBase : IAsyncLifetime
{
    protected readonly string databaseName;
    protected readonly Fixture fixture;
    protected readonly DataAccessMockHelper accessMockHelper;
    protected readonly LeagueDbContext dbContext;
    
    protected ILeagueProvider LeagueProvider => accessMockHelper.LeagueProvider;

    protected const object? defaultId = null;
    protected long TestLeagueId => dbContext.Leagues.IgnoreQueryFilters().First().Id;
    protected string TestLeagueName => dbContext.Leagues.IgnoreQueryFilters().First().Name;
    protected long TestSeasonId => dbContext.Seasons.IgnoreQueryFilters().First().SeasonId;
    protected long TestScoringId => dbContext.Scorings.IgnoreQueryFilters().First().ScoringId;
    protected long TestScheduleId => dbContext.Schedules.IgnoreQueryFilters().First().ScheduleId;
    protected long TestEventId => dbContext.Events.IgnoreQueryFilters().First().EventId;
    protected long TestSessionId => dbContext.Sessions.IgnoreQueryFilters().First().SessionId;
    protected long TestResultId => dbContext.ScoredEventResults.IgnoreQueryFilters().First().ResultId;
    protected long TestPointRuleId => dbContext.PointRules.IgnoreQueryFilters().First().PointRuleId;
    protected long TestResultConfigId => dbContext.ResultConfigurations.IgnoreQueryFilters().First().ResultConfigId;
    protected long TestReviewId => dbContext.IncidentReviews.IgnoreQueryFilters().First().ReviewId;
    protected long TestMemberId => dbContext.Members.IgnoreQueryFilters().First().Id;
    protected long TestMemberId2 => dbContext.Members.Skip(1).IgnoreQueryFilters().First().Id;
    protected long TestCommentId => dbContext.ReviewComments.IgnoreQueryFilters().First().CommentId;
    protected long TestReviewVoteId => dbContext.AcceptedReviewVotes.IgnoreQueryFilters().First().ReviewVoteId;
    protected long TestVoteCategory => dbContext.VoteCategories.IgnoreQueryFilters().First().CatId;
    protected const string testLeagueName = "TestLeague";
    protected const string testUserName = "TestUser";
    protected const string testUserId = "a0031cbe-a28b-48ac-a6db-cdca446a8162";
    protected static readonly IEnumerable<string> testLeagueRoles = new string[] { LeagueRoles.Member };

    public DataAccessTestsBase()
    {
        fixture = new Fixture();
        accessMockHelper = new();
        databaseName = fixture.Create<string>();
        dbContext = accessMockHelper.CreateMockDbContext(databaseName);
        fixture.Register(() => dbContext);
    }

    public virtual async Task InitializeAsync()
    {
        dbContext.Database.EnsureCreated();
        await accessMockHelper.PopulateBasicTestSet(dbContext);
        accessMockHelper.SetCurrentLeague(await dbContext.Leagues.FirstAsync());
    }

    public virtual async Task DisposeAsync()
    {
        dbContext.Database.EnsureDeleted();
        await dbContext.DisposeAsync();
    }
}
