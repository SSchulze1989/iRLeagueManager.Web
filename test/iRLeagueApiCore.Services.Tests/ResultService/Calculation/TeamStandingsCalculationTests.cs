using AutoFixture.Dsl;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using System.Diagnostics;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class TeamStandingCalculationServiceTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public async Task Calculate_ShouldNotThrow()
    {
        var data = GetCalculationData();
        var config = CalculationConfigurationBuilder(data.LeagueId, data.EventId).Create();
        var sut = CreateSut(config);

        await sut.Calculate(data);
    }

    [Fact]
    public async Task Calculate_ShouldAccumulateTeamRows()
    {
        const int nEvents = 3;
        const int nRaces = 2;
        var teamId = fixture.Create<long>();
        var testRowData = TestRowBuilder()
            .With(x => x.TeamId, teamId)
            .CreateMany(nEvents * nRaces);
        var data = CalculationDataBuilder(nEvents, nRaces, true).Create();
        var tmp = data.PreviousEventResults.SelectMany(x => x.SessionResults).Concat(data.CurrentEventResult.SessionResults)
            .Zip(testRowData);
        Debug.Assert(tmp.None(x => x.First.ResultRows.Any(y => y.TeamId == teamId)));
        foreach (var (result, rowData) in tmp)
        {
            //result.ResultRows = result.ResultRows.Concat(new[] { rowData });
            result.ResultRows = new[] { rowData };
        }
        var config = CalculationConfigurationBuilder(data.LeagueId, data.EventId)
            .With(x => x.ResultKind, Common.Enums.ResultKind.Team)
            .With(x => x.UseCombinedResult, false)
            .With(x => x.WeeksCounted, nEvents)
            .Create();
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        test.StandingRows.Should().ContainSingle(x => x.TeamId == teamId);
        var testRow = test.StandingRows.Single(x => x.TeamId == teamId);
        var lastResultRowData = testRowData.TakeLast(2);
        testRow.MemberId.Should().BeNull();
        testRow.Wins.Should().Be(testRowData.Sum(x => x.FinalPosition == 1 ? 1 : 0));
        testRow.RacePoints.Should().Be((int)testRowData.Sum(x => x.RacePoints + x.BonusPoints));
        testRow.TotalPoints.Should().Be((int)testRowData.Sum(x => x.RacePoints + x.BonusPoints - x.PenaltyPoints));
        testRow.CompletedLaps.Should().Be((int)testRowData.Sum(x => x.CompletedLaps));
        testRow.CompletedLapsChange.Should().Be((int)lastResultRowData.Sum(x => x.CompletedLaps));
        testRow.DroppedResultCount.Should().Be(0);
        testRow.Incidents.Should().Be((int)testRowData.Sum(x => x.Incidents));
        testRow.IncidentsChange.Should().Be((int)lastResultRowData.Sum(x => x.Incidents));
        testRow.LeadLaps.Should().Be((int)testRowData.Sum(x => x.LeadLaps));
        testRow.LeadLapsChange.Should().Be((int)lastResultRowData.Sum(x => x.LeadLaps));
        testRow.PenaltyPoints.Should().Be((int)testRowData.Sum(x => x.PenaltyPoints));
        testRow.PenaltyPointsChange.Should().Be((int)lastResultRowData.Sum(x => x.PenaltyPoints));
        testRow.PolePositions.Should().Be(testRowData.Count(x => x.StartPosition == 1));
        testRow.PolePositionsChange.Should().Be(lastResultRowData.Count(x => x.StartPosition == 1));
        testRow.Races.Should().Be(testRowData.Count());
        testRow.RacesCounted.Should().Be(testRowData.Count());
        testRow.Top10.Should().Be(testRowData.Count(x => x.FinalPosition <= 10));
        testRow.Top5.Should().Be(testRowData.Count(x => x.FinalPosition <= 5));
        testRow.Top3.Should().Be(testRowData.Count(x => x.FinalPosition <= 3));
    }

    [Fact]
    public async Task Calculate_ShouldOnlyUseLastSessionResult_WhenCombinedResultConfigure()
    {
        const int nEvents = 3;
        const int nRaces = 2;
        var teamId = fixture.Create<long>();
        var testRowData = TestRowBuilder()
            .With(x => x.TeamId, teamId)
            .CreateMany(nEvents * (nRaces + 1));
        var data = CalculationDataBuilder(nEvents, nRaces + 1, false).Create();
        var tmp = data.PreviousEventResults.SelectMany(x => x.SessionResults.OrderBy(x => x.SessionNr)).Concat(data.CurrentEventResult.SessionResults.OrderBy(x => x.SessionNr))
            .Zip(testRowData);
        foreach (var (result, rowData) in tmp)
        {
            //result.ResultRows = result.ResultRows.Concat(new[] { rowData });
            result.ResultRows = new[] { rowData };
        }
        var config = CalculationConfigurationBuilder(data.LeagueId, data.EventId)
            .With(x => x.ResultKind, Common.Enums.ResultKind.Team)
            .With(x => x.UseCombinedResult, true)
            .With(x => x.WeeksCounted, nEvents)
            .Create();
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        test.StandingRows.Should().ContainSingle(x => x.TeamId == teamId);
        var testRow = test.StandingRows.Single(x => x.TeamId == teamId);
        var combinedResultRowData = testRowData.Chunk(3).Select(x => x.Last()).ToList();
        var lastResultRowData = combinedResultRowData.TakeLast(1);
        testRow.Wins.Should().Be(combinedResultRowData.Sum(x => x.FinalPosition == 1 ? 1 : 0));
        testRow.RacePoints.Should().Be((int)combinedResultRowData.Sum(x => x.RacePoints + x.BonusPoints));
        testRow.TotalPoints.Should().Be((int)combinedResultRowData.Sum(x => x.RacePoints + x.BonusPoints - x.PenaltyPoints));
        testRow.CompletedLaps.Should().Be((int)combinedResultRowData.Sum(x => x.CompletedLaps));
        testRow.CompletedLapsChange.Should().Be((int)lastResultRowData.Sum(x => x.CompletedLaps));
        testRow.DroppedResultCount.Should().Be(0);
        testRow.Incidents.Should().Be((int)combinedResultRowData.Sum(x => x.Incidents));
        testRow.IncidentsChange.Should().Be((int)lastResultRowData.Sum(x => x.Incidents));
        testRow.LeadLaps.Should().Be((int)combinedResultRowData.Sum(x => x.LeadLaps));
        testRow.LeadLapsChange.Should().Be((int)lastResultRowData.Sum(x => x.LeadLaps));
        testRow.PenaltyPoints.Should().Be((int)combinedResultRowData.Sum(x => x.PenaltyPoints));
        testRow.PenaltyPointsChange.Should().Be((int)lastResultRowData.Sum(x => x.PenaltyPoints));
        testRow.PolePositions.Should().Be(combinedResultRowData.Count(x => x.StartPosition == 1));
        testRow.PolePositionsChange.Should().Be(lastResultRowData.Count(x => x.StartPosition == 1));
        testRow.Races.Should().Be(combinedResultRowData.Count());
        testRow.RacesCounted.Should().Be(combinedResultRowData.Count());
        testRow.Top10.Should().Be(combinedResultRowData.Count(x => x.FinalPosition <= 10));
        testRow.Top5.Should().Be(combinedResultRowData.Count(x => x.FinalPosition <= 5));
        testRow.Top3.Should().Be(combinedResultRowData.Count(x => x.FinalPosition <= 3));
    }

    [Fact]
    public async Task Calculate_ShouldAssignPositions_ByTotalPointsAndPenaltyPoints()
    {
        var nEvents = 3;
        var nRaces = 1;
        var data = CalculationDataBuilder(nEvents, nRaces, false).Create();
        // make sure two rows have same total points
        var racePoints = fixture.Create<double>();
        var bonusPoints = fixture.Create<double>();
        var penaltyPoints = fixture.Create<double>();
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(0).RacePoints = racePoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(1).RacePoints = racePoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(0).BonusPoints = bonusPoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(1).BonusPoints = bonusPoints + penaltyPoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(0).PenaltyPoints = 0.0;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(1).PenaltyPoints = penaltyPoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(0).TotalPoints = racePoints + bonusPoints;
        data.CurrentEventResult.SessionResults.First().ResultRows.ElementAt(1).TotalPoints = racePoints + bonusPoints - penaltyPoints;
        var config = CalculationConfigurationBuilder(data.LeagueId, data.EventId)
            .With(x => x.ResultKind, ResultKind.Member)
            .With(x => x.WeeksCounted, nEvents)
            .With(x => x.SortOptions, new[]
            {
                SortOptions.TotalPtsDesc,
                SortOptions.PenPtsAsc,
            })
            .Create();
        var sut = CreateSut(config);

        var test = await sut.Calculate(data);

        var testOrder = test.StandingRows
            .OrderByDescending(x => x.TotalPoints)
            .ThenBy(x => x.PenaltyPoints);
        var positions = Enumerable.Range(1, testOrder.Count());
        testOrder.Should().BeInAscendingOrder(x => x.Position);
        testOrder.Select(x => x.Position).Should().BeEquivalentTo(positions);
        test.StandingRows.Should().BeEquivalentTo(testOrder);
    }

    private TeamStandingCalculationService CreateSut(StandingCalculationConfiguration config)
    {
        if (config != null)
        {
            fixture.Register(() => config);
        }
        return fixture.Create<TeamStandingCalculationService>();
    }

    private StandingCalculationData GetCalculationData()
    {
        return fixture.Create<StandingCalculationData>();
    }

    private IPostprocessComposer<StandingCalculationData> CalculationDataBuilder(int nEvents = 3, int nRacesPerEvent = 3, bool hasCombinedResult = false)
    {
        return fixture.Build<StandingCalculationData>()
            .With(x => x.PreviousEventResults, () => EventResultDataBuilder(nRacesPerEvent, hasCombinedResult).CreateMany(nEvents - 1).ToList())
            .With(x => x.CurrentEventResult, () => EventResultDataBuilder(nRacesPerEvent, hasCombinedResult).Create());
    }

    private IPostprocessComposer<EventCalculationResult> EventResultDataBuilder(int nRaces = 2, bool hasCombinedResult = false)
    {
        return fixture.Build<EventCalculationResult>()
            .With(x => x.SessionResults, () => SessionResultDataBuilder().CreateMany(nRaces));
    }

    private IPostprocessComposer<SessionCalculationResult> SessionResultDataBuilder()
    {
        return fixture.Build<SessionCalculationResult>();
    }

    private IPostprocessComposer<StandingCalculationConfiguration> CalculationConfigurationBuilder(long leagueId, long eventId, 
        ChampSeasonEntity? champSeason = null)
    {
        return fixture.Build<StandingCalculationConfiguration>()
            .With(x => x.LeagueId, leagueId)
            .With(x => x.EventId, eventId)
            .With(x => x.ResultConfigs, champSeason?.ResultConfigurations.Select(x => x.ResultConfigId) ?? Array.Empty<long>());
    }

    private IPostprocessComposer<ResultRowCalculationResult> TestRowBuilder()
    {
        return fixture.Build<ResultRowCalculationResult>()
            .Without(x => x.AddPenalties)
            .Do(x => { x.TotalPoints = x.RacePoints + x.BonusPoints - x.PenaltyPoints; });
    }

    private IEnumerable<ResultRowCalculationData> GetTestRows()
    {
        return TestRowBuilder().CreateMany();
    }
}

