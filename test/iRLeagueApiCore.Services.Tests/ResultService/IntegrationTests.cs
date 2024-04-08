using Aydsko.iRacingData.Stats;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Excecution;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using K4os.Hash.xxHash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace iRLeagueApiCore.Services.Tests.ResultService;
public sealed class IntegrationTests : DataAccessTestsBase
{
    private readonly IServiceProvider services;

    public IntegrationTests()
    {
        var services = new ServiceCollection() as IServiceCollection;
        services.AddScoped(x => dbContext);
        services.AddLogging();
        services.AddBackgroundQueue();
        services.AddResultService();
        this.services = services.BuildServiceProvider();
    }

    [Fact]
    public async Task CalculateSingleHeaderRace()
    {
        var schedule = await dbContext.Schedules
            .FirstAsync();
        var @event = accessMockHelper.EventBuilder(schedule).Create();
        @event.Sessions = new[]
        {
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Practice),
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Qualifying),
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Race),
        }.ToList();
        var config = accessMockHelper.CreateConfiguration(@event);
        var condition = fixture.Build<FilterConditionModel>()
            .With(x => x.ColumnPropertyName, nameof(ResultRowCalculationResult.FinishPosition))
            .With(x => x.FilterValues, new[] {"3"})
            .With(x => x.Comparator, Common.Enums.ComparatorType.IsSmallerOrEqual)
            .With(x => x.Action, Common.Enums.MatchedValueAction.Keep)
            .Create();
        var filter = fixture.Build<FilterOptionEntity>()
            .With(x => x.Conditions, new[]
            {
                    condition,
            })
            .Without(x => x.ChampSeason)
            .Without(x => x.PointFilterResultConfig)
            .Without(x => x.ResultFilterResultConfig)
            .Create();
        config.PointFilters.Add(filter);

        var members = await dbContext.LeagueMembers
            .Take(5)
            .ToListAsync();
        var result = accessMockHelper.CreateResult(@event, members);
        @event.EventResult = result;
        @event.ScoredEventResults.Clear();
        @event.ResultConfigs.Add(config);
        dbContext.Events.Add(@event);
        dbContext.FilterOptions.Add(filter);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        var sut = services.GetRequiredService<ExecuteEventResultCalculation>();
        await sut.Execute(@event.EventId);

        var testResult = await dbContext.ScoredSessionResults
            .Where(x => x.ScoredEventResult.EventId == @event.EventId)
            .OrderByDescending(x => x.SessionNr)
            .Include(x => x.ScoredResultRows)
                .ThenInclude(x => x.Member)
            .FirstAsync();

        var compareResult = result.SessionResults
            .Last();
        testResult.ScoredResultRows.Should().HaveSameCount(members);
        testResult.ScoredResultRows.Select(x => x.FinishPosition).Should()
            .BeEquivalentTo(Enumerable.Range(1, 5));
        testResult.ScoredResultRows.Select(x => x.StartPosition).Should()
            .BeEquivalentTo(Enumerable.Range(1, 5));
        testResult.ScoredResultRows.Select(x => x.PointsEligible).Should()
            .BeEquivalentTo(compareResult.ResultRows.Select(x => x.FinishPosition <= 3));
        testResult.ScoredResultRows.Select(x => x.RacePoints).Should()
            .BeEquivalentTo(testResult.ScoredResultRows.Select(x => x.PointsEligible ? x.RacePoints : 0.0));
    }

    [Fact]
    public async Task CalculateMultiHeaderCombinedRace()
    {
        var schedule = await dbContext.Schedules
            .FirstAsync();
        var @event = accessMockHelper.EventBuilder(schedule).Create();
        @event.Sessions = new[]
        {
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Practice),
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Qualifying),
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Race),
            accessMockHelper.CreateSession(@event, Common.Enums.SessionType.Race),
        }.ToList();
        @event.Sessions
            .Select((x, i) => new { Session = x, Index = i })
            .ToList()
            .ForEach(x => x.Session.SessionNr = x.Index + 1);
        var config = accessMockHelper.CreateConfiguration(@event);
        var combinedScoring = fixture.Build<ScoringEntity>()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.Index, 999)
            .With(x => x.IsCombinedResult, true)
            .With(x => x.PointsRule, () => fixture.Build<PointRuleEntity>()
                .With(x => x.LeagueId, @event.LeagueId)
                .Without(x => x.Scorings)
                .Without(x => x.AutoPenalties)
                .Without(x => x.League)
                .Without(x => x.BonusPoints)
                .Create())
            .Without(x => x.DependendScorings)
            .Without(x => x.ExtScoringSource)
            .Without(x => x.ResultConfiguration)
            .Create();
        var condition = fixture.Build<FilterConditionModel>()
            .With(x => x.ColumnPropertyName, nameof(ResultRowCalculationResult.FinishPosition))
            .With(x => x.FilterValues, new[] { "3" })
            .With(x => x.Comparator, Common.Enums.ComparatorType.IsSmallerOrEqual)
            .With(x => x.Action, Common.Enums.MatchedValueAction.Keep)
            .Create();
        var filter = fixture.Build<FilterOptionEntity>()
            .With(x => x.Conditions, new[]
            {
                    condition,
            })
            .Without(x => x.ChampSeason)
            .Without(x => x.PointFilterResultConfig)
            .Without(x => x.ResultFilterResultConfig)
            .Create();
        config.Scorings.Add(combinedScoring);
        config.PointFilters.Add(filter);

        var members = await dbContext.LeagueMembers
            .Take(5)
            .ToListAsync();
        var result = accessMockHelper.CreateResult(@event, members);
        @event.EventResult = result;
        @event.ScoredEventResults.Clear();
        @event.ResultConfigs.Add(config);
        dbContext.Events.Add(@event);
        dbContext.FilterOptions.Add(filter);
        dbContext.ResultConfigurations.Add(config);
        await dbContext.SaveChangesAsync();

        var sut = services.GetRequiredService<ExecuteEventResultCalculation>();
        await sut.Execute(@event.EventId);

        var testResult = await dbContext.ScoredSessionResults
            .Where(x => x.ScoredEventResult.EventId == @event.EventId)
            .OrderByDescending(x => x.SessionNr)
            .Include(x => x.ScoredResultRows)
                .ThenInclude(x => x.Member)
            .FirstAsync();

        var compareResult = result.SessionResults
            .Last();
        testResult.SessionNr.Should().Be(999);
        testResult.ScoredResultRows.Should().HaveSameCount(members);
        testResult.ScoredResultRows.Select(x => x.FinishPosition).Should()
            .BeEquivalentTo(compareResult.ResultRows.Select(x => x.FinishPosition));
        testResult.ScoredResultRows.Select(x => x.StartPosition).Should()
            .BeEquivalentTo(compareResult.ResultRows.Select(x => x.StartPosition));
        testResult.ScoredResultRows.Select(x => x.PointsEligible).Should()
            .BeEquivalentTo(compareResult.ResultRows.Select(x => x.FinishPosition <= 3));
        testResult.ScoredResultRows.Select(x => x.RacePoints).Should()
            .BeEquivalentTo(testResult.ScoredResultRows.Select(x => x.PointsEligible ? x.RacePoints : 0.0));
    }

    private async Task<EventEntity> GetFirstEventEntity()
    {
        return await dbContext.Events
            .Include(x => x.EventResult)
                .ThenInclude(x => x.SessionResults)
                    .ThenInclude(x => x.ResultRows)
            .Include(x => x.Schedule.Season.League)
            .Include(x => x.Sessions)
                .ThenInclude(x => x.IncidentReviews)
            .FirstAsync();
    }

    private EventCalculationConfiguration GetConfiguration(EventEntity @event)
    {
        return fixture.Build<EventCalculationConfiguration>()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.EventId, @event.EventId)
            .Without(x => x.SourceResultConfigId)
            .Create();
    }

    private static ILogger<T> CreateLogger<T>()
    {
        return Mock.Of<ILogger<T>>();
    }
}
