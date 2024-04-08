using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.Tests.ResultService.DataAccess;

public sealed class StandingCalculationConfigurationProviderTests : DataAccessTestsBase
{
    [Theory]
    [InlineData(default(long))]
    [InlineData(-42)]
    public async Task GetConfiguration_ShouldProvideEmptyConfiguration_WhenEventDoesNotExist(long? eventId)
    {
        var season = await GetFirstSeasonAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, eventId, null);

        test.LeagueId.Should().Be(0);
        test.SeasonId.Should().Be(0);
        test.EventId.Should().Be(0);
        test.ResultConfigs.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConfiguration_ShouldConfigurationForEvent_WhenEventIdIsNotNull()
    {
        var season = await GetFirstSeasonAsync();
        var @event = season.Schedules.First().Events.First();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, @event.EventId, null);

        test.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.EventId.Should().Be(@event.EventId);
        test.ResultConfigs.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideConfigurationForLatestEvent_WhenEventIdIsNull()
    {
        var season = await GetFirstSeasonAsync();
        // create results for first two events
        var events = season.Schedules.SelectMany(x => x.Events).OrderBy(x => x.Date);
        foreach (var @event in events.Take(2))
        {
            var result = accessMockHelper.CreateScoredResult(@event, null);
            @event.ScoredEventResults.Add(result);
            dbContext.ScoredEventResults.Add(result);
        }
        var latestEvent = events.ElementAt(1);
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, null, null);

        test.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.EventId.Should().Be(latestEvent.EventId);
        test.ResultConfigs.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideConfiguration_WithStandingConfigId()
    {
        var season = await GetFirstSeasonAsync();
        var @event = season.Schedules.First().Events.First();
        var championship = await GetFirstChampionshipAsync();
        var config = accessMockHelper.CreateConfiguration(@event);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var standingConfig = accessMockHelper.CreateStandingConfiguration(season.League);
        dbContext.ChampSeasons.Add(champSeason);
        dbContext.StandingConfigurations.Add(standingConfig);
        dbContext.ResultConfigurations.Add(config);
        champSeason.StandingConfiguration = standingConfig;
        champSeason.ResultConfigurations = new[] { config };
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, @event.EventId, standingConfig.StandingConfigId);

        test.LeagueId.Should().Be(season.LeagueId);
        test.SeasonId.Should().Be(season.SeasonId);
        test.EventId.Should().Be(@event.EventId);
        test.ResultConfigs.Should().HaveCount(1);
        test.ResultConfigs.First().Should().Be(champSeason.ResultConfigurations.First().ResultConfigId);
        test.StandingConfigId.Should().Be(standingConfig.StandingConfigId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideConfiguration_WithMultipleResultConfigurations()
    {
        var season = await GetFirstSeasonAsync();
        var @event = season.Schedules.First().Events.First();
        var championship = await GetFirstChampionshipAsync();
        int configCount = 2;
        var configs = accessMockHelper.ConfigurationBuilder(@event).CreateMany(configCount);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var standingConfig = accessMockHelper.CreateStandingConfiguration(season.League);
        dbContext.ChampSeasons.Add(champSeason);
        dbContext.StandingConfigurations.Add(standingConfig);
        dbContext.ResultConfigurations.AddRange(configs);
        champSeason.StandingConfiguration = standingConfig;
        champSeason.ResultConfigurations = configs.ToList();
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, @event.EventId, standingConfig.StandingConfigId);

        test.ResultConfigs.Should().HaveCount(configCount);
        test.ResultConfigs.First().Should().Be(champSeason.ResultConfigurations.First().ResultConfigId);
        test.StandingConfigId.Should().Be(standingConfig.StandingConfigId);
    }

    [Fact]
    public async Task GetConfiguration_ShouldProvideConfiguration_WithChampSeason()
    {
        var season = await GetFirstSeasonAsync();
        var @event = season.Schedules.First().Events.First();
        var championship = await GetFirstChampionshipAsync();
        var config = accessMockHelper.CreateConfiguration(@event);
        var champSeason = accessMockHelper.CreateChampSeason(championship, season);
        var standingConfig = accessMockHelper.CreateStandingConfiguration(season.League);
        dbContext.ChampSeasons.Add(champSeason);
        dbContext.StandingConfigurations.Add(standingConfig);
        dbContext.ResultConfigurations.Add(config);
        champSeason.StandingConfiguration = standingConfig;
        champSeason.ResultConfigurations = new[] { config };
        await dbContext.SaveChangesAsync();
        var sut = CreateSut();

        var test = await sut.GetConfiguration(season.SeasonId, @event.EventId, standingConfig.StandingConfigId);

        test.ChampSeasonId.Should().Be(champSeason.ChampSeasonId);
        test.Name.Should().Be(champSeason.Championship.Name);
        test.DisplayName.Should().Be(champSeason.Championship.DisplayName);
    }

    private StandingCalculationConfigurationProvider CreateSut()
    {
        return fixture.Create<StandingCalculationConfigurationProvider>();
    }

    private async Task<SeasonEntity> GetFirstSeasonAsync()
    {
        return await dbContext.Seasons
            .Include(x => x.League)
            .Include(x => x.Schedules)
                .ThenInclude(x => x.Events)
                    .ThenInclude(x => x.ScoredEventResults)
            .Include(x => x.ChampSeasons)
            .FirstAsync();
    }

    private async Task<ChampionshipEntity> GetFirstChampionshipAsync()
    {
        return await dbContext.Championships
            .Include(x => x.ChampSeasons)
            .FirstAsync();
    }
}
