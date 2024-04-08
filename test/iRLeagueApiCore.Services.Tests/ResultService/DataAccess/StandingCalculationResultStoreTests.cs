using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.Tests.ResultService.DataAccess;
public sealed class StandingCalculationResultStoreTests : DataAccessTestsBase
{
    [Fact]
    public async Task StoreCalculationResult_ShouldStoreResult_WithChampSeason()
    {
        var @event = await GetFirstEvent();
        var championship = await dbContext.Championships.FirstAsync();
        var champSeason = accessMockHelper.CreateChampSeason(championship, @event.Schedule.Season);
        var standingConfig = accessMockHelper.CreateStandingConfiguration(@event.Schedule.Season.League);
        champSeason.StandingConfiguration = standingConfig;
        dbContext.ChampSeasons.Add(champSeason);
        dbContext.StandingConfigurations.Add(standingConfig);
        await dbContext.SaveChangesAsync();
        var result = GetCalculationResult(@event, standingConfig);
        var sut = CreateSut();

        await sut.StoreCalculationResult(result);

        var test = await dbContext.Standings
            .Where(x => x.EventId == @event.EventId)
            .Where(x => x.StandingConfigId == standingConfig.StandingConfigId)
            .FirstOrDefaultAsync();
        test.Should().NotBeNull();
        test!.ChampSeasonId.Should().Be(champSeason.ChampSeasonId);
    }

    private StandingCalculationResultStore CreateSut()
    {
        return new StandingCalculationResultStore(dbContext);
    }

    private StandingCalculationResult GetCalculationResult(EventEntity @event, StandingConfigurationEntity? standingConfig)
    {
        return fixture.Build<StandingCalculationResult>()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.SeasonId, @event.Schedule.SeasonId)
            .With(x => x.EventId, @event.EventId)
            .With(x => x.StandingConfigId, standingConfig?.StandingConfigId)
            .With(x => x.ChampSeasonId, standingConfig?.ChampSeasons.FirstOrDefault(x => x.SeasonId == @event.Schedule.SeasonId)?.ChampSeasonId)
            .Without(x => x.StandingRows)
            .Create();
    }

    private async Task<EventEntity> GetFirstEvent()
    {
        return await dbContext.Events
            .Include(x => x.Schedule.Season)
                .ThenInclude(x => x.League)
            .FirstAsync();
    }
}

