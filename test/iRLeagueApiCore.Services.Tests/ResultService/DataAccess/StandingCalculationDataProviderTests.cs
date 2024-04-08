using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.Tests.ResultService.DataAccess;

public sealed class StandingCalculationDataProviderTests : DataAccessTestsBase
{
    [Fact]
    public async Task GetData_ShouldProvidePreviousResults_WithDefaultResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        AddMultipleScoredEventResults(events, null, prevCount + 1);
        await dbContext.SaveChangesAsync();
        var config = CreateStandingConfiguration(season, events.ElementAt(prevCount));
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.PreviousEventResults.Should().HaveCount(prevCount);
        foreach (var (result, prevEvent) in test.PreviousEventResults.Zip(events.Take(prevCount)))
        {
            result.EventId.Should().Be(prevEvent.EventId);
            result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(prevEvent.Sessions.Count);
        }
    }

    [Fact]
    public async Task GetData_ShouldProvideCurrentResult_WithDefaultResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        AddMultipleScoredEventResults(events, null, prevCount + 1);
        await dbContext.SaveChangesAsync();
        var @event = events.ElementAt(prevCount);
        var config = CreateStandingConfiguration(season, @event);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.EventId.Should().Be(@event.EventId);
        var result = test.CurrentEventResult;
        result.EventId.Should().Be(@event.EventId);
        result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(@event.Sessions.Count);
    }

    [Fact]
    public async Task GetData_ShouldProvidePreviousResults_WithSingleResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var resultConfig = champSeason.ResultConfigurations.First();
        AddMultipleScoredEventResults(events, resultConfig, prevCount + 1);
        await dbContext.SaveChangesAsync();
        var config = CreateStandingConfiguration(season, events.ElementAt(prevCount), champSeason);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.PreviousEventResults.Should().HaveCount(prevCount);
        foreach (var (result, prevEvent) in test.PreviousEventResults.Zip(events.Take(prevCount)))
        {
            result.EventId.Should().Be(prevEvent.EventId);
            result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(prevEvent.Sessions.Count);
        }
    }

    [Fact]
    public async Task GetData_ShouldProvideCurrentResult_WithSingleResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var resultConfig = champSeason.ResultConfigurations.First();
        AddMultipleScoredEventResults(events, resultConfig, prevCount + 1);
        await dbContext.SaveChangesAsync();
        var @event = events.ElementAt(prevCount);
        var config = CreateStandingConfiguration(season, @event, champSeason);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.EventId.Should().Be(@event.EventId);
        var result = test.CurrentEventResult;
        result.EventId.Should().Be(@event.EventId);
        result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(@event.Sessions.Count);
    }

    [Fact]
    public async Task GetData_ShouldProvidePreviousResults_WithMultipleResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var resultConfigs = champSeason.ResultConfigurations = accessMockHelper.ConfigurationBuilder(events.First()).CreateMany(2).ToList();
        AddMultipleScoredEventResults(events.Take(1), resultConfigs.First(), 1);
        AddMultipleScoredEventResults(events.Skip(1), resultConfigs.Last(), prevCount);
        await dbContext.SaveChangesAsync();
        var config = CreateStandingConfiguration(season, events.ElementAt(prevCount), champSeason);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.PreviousEventResults.Should().HaveCount(prevCount);
        foreach (var (result, prevEvent) in test.PreviousEventResults.Zip(events.Take(prevCount)))
        {
            result.EventId.Should().Be(prevEvent.EventId);
            result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(prevEvent.Sessions.Count);
        }
    }

    [Fact]
    public async Task GetData_ShouldProvideCurrentResult_WithMultipleResultConfig()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var resultConfigs = champSeason.ResultConfigurations = accessMockHelper.ConfigurationBuilder(events.First()).CreateMany(2).ToList();
        AddMultipleScoredEventResults(events.Take(1), resultConfigs.First(), 1);
        AddMultipleScoredEventResults(events.Skip(1), resultConfigs.Last(), prevCount);
        await dbContext.SaveChangesAsync();
        var @event = events.ElementAt(prevCount);
        var config = CreateStandingConfiguration(season, @event, champSeason);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.EventId.Should().Be(@event.EventId);
        var result = test.CurrentEventResult;
        result.EventId.Should().Be(@event.EventId);
        result.SessionResults.Should().HaveCountGreaterThanOrEqualTo(@event.Sessions.Count);
    }

    [Fact]
    public async Task GetData_ShouldReturnNull_WhenNoResultFound()
    {
        var season = await GetFirstSeasonAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        var @event = events.ElementAt(0);
        var config = CreateStandingConfiguration(season, @event);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().BeNull();
    }

    [Fact]
    public async Task GetData_ShouldProvideOnlyPreviousResults_WhenNoCurrentEventResultFound()
    {
        var prevCount = 2;
        var season = await GetFirstSeasonAsync();
        var championship = await dbContext.Championships.FirstAsync();
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        AddMultipleScoredEventResults(events, null, prevCount);
        await dbContext.SaveChangesAsync();
        var @event = events.ElementAt(prevCount);
        var config = CreateStandingConfiguration(season, @event);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.EventId.Should().Be(@event.EventId);
        test.PreviousEventResults.Should().HaveCount(prevCount);
        test.CurrentEventResult.EventId.Should().Be(@event.EventId);
        test.CurrentEventResult.SessionResults.Should().BeEmpty();
    }

    private async Task<SeasonEntity> GetFirstSeasonAsync()
    {
        return await dbContext.Seasons
            .Include(x => x.League)
            .Include(x => x.Schedules)
                .ThenInclude(x => x.Events)
                    .ThenInclude(x => x.ScoredEventResults)
            .FirstAsync();
    }

    private void AddMultipleScoredEventResults(IEnumerable<EventEntity> events, ResultConfigurationEntity? config, int count)
    {
        foreach (var @event in events.Take(count))
        {
            var result = accessMockHelper.CreateScoredResult(@event, config);
            @event.ScoredEventResults.Add(result);
            dbContext.ScoredEventResults.Add(result);
        }
    }

    private StandingCalculationConfiguration CreateStandingConfiguration(SeasonEntity season, EventEntity @event, 
        ChampSeasonEntity? champSeason = null)
    {
        return fixture.Build<StandingCalculationConfiguration>()
            .With(x => x.LeagueId, season.LeagueId)
            .With(x => x.SeasonId, season.SeasonId)
            .With(x => x.EventId, @event.EventId)
            .With(x => x.ResultConfigs, champSeason?.ResultConfigurations.Select(x => x.ResultConfigId) ?? Array.Empty<long>())
            .With(x => x.WeeksCounted, 2)
            .Create();
    }

    private StandingCalculationDataProvider CreateSut()
    {
        return fixture.Create<StandingCalculationDataProvider>();
    }
}
