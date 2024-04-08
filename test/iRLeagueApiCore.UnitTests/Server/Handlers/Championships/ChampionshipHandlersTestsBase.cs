using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore.Models;
using MediatR;
using Xunit.Sdk;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Championships;
public abstract class ChampionshipHandlersTestsBase<THandler, TRequest, TResult> : HandlersTestsBase<THandler, TRequest, TResult>
    where THandler : IRequestHandler<TRequest, TResult>
    where TRequest : class, IRequest<TResult>
{
    protected long TestChampionshipId => dbContext.Championships.First().ChampionshipId;
    protected long TestChampSeasonId => dbContext.ChampSeasons.First().ChampSeasonId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var league = dbContext.Leagues.First();
        var championships = dbContext.Championships;

        var season = dbContext.Seasons.First();
        foreach (var championship in championships)
        {
            var champSeason = accessMockHelper.CreateChampSeason(championship, season, nResultConfigs: 2);
            dbContext.ChampSeasons.Add(champSeason);
        }
        var season2 = accessMockHelper.CreateSeason(league);
        dbContext.Seasons.Add(season2);

        await dbContext.SaveChangesAsync();
    }

    protected void CompareChampionshipEntity(ChampionshipEntity test, ChampionshipEntity expected)
    {
        test.Name.Should().Be(expected.Name);
        test.DisplayName.Should().Be(expected.DisplayName);
    }

    protected void CompareChampSeasonEntity(ChampSeasonEntity test, ChampSeasonEntity expected)
    {
        test.ChampionshipId.Should().Be(expected.ChampionshipId);
        test.SeasonId.Should().Be(expected.SeasonId);
        CompareStandingConfigurationEntity(test.StandingConfiguration, expected.StandingConfiguration);
        test.ResultConfigurations.Should().HaveSameCount(expected.ResultConfigurations);
        test.ResultConfigurations.Zip(expected.ResultConfigurations)
            .ToList()
            .ForEach(x => CompareResultConfigurationEntity(x.First, x.Second));
    }

    protected void CompareStandingConfigurationEntity(StandingConfigurationEntity test, StandingConfigurationEntity expected)
    {
        test.ResultKind.Should().Be(expected.ResultKind);
        test.UseCombinedResult.Should().Be(expected.UseCombinedResult);
        test.WeeksCounted.Should().Be(expected.WeeksCounted);
    }

    protected void CompareResultConfigurationEntity(ResultConfigurationEntity test, ResultConfigurationEntity expected)
    {
        test.PointFilters.Should().HaveSameCount(expected.PointFilters);
        test.PointFilters.Zip(expected.PointFilters)
            .ToList()
            .ForEach(x => CompareFilterOptionEntity(x.First, x.Second));
        test.ResultFilters.Should().HaveSameCount(expected.ResultFilters);
        test.ResultFilters.Zip(expected.ResultFilters)
            .ToList()
            .ForEach(x => CompareFilterOptionEntity(x.First, x.Second));
    }

    protected void CompareFilterOptionEntity(FilterOptionEntity test, FilterOptionEntity expected)
    {
        test.Conditions.Should().HaveSameCount(expected.Conditions);
        test.Conditions.Zip(expected.Conditions)
            .ToList()
            .ForEach(x => CompareFilterConditionEntity(x.First, x.Second));
    }

    protected void CompareFilterConditionEntity(FilterConditionModel test, FilterConditionModel expected)
    {
        test.FilterType.Should().Be(expected.FilterType);
        test.FilterValues.Should().BeEquivalentTo(expected.FilterValues);
        test.Comparator.Should().Be(expected.Comparator);
        test.Action.Should().Be(expected.Action);
    }
}
