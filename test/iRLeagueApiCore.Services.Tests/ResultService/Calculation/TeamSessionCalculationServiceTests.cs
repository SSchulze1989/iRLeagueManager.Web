using AutoFixture.Dsl;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueApiCore.Mocking.Extensions;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class TeamSessionCalculationServiceTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public async Task Calculate_ShouldSetResultMetaData()
    {
        var data = GetCalculationData();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.LeagueId.Should().Be(config.LeagueId);
        test.Name.Should().Be(config.Name);
        test.SessionId.Should().Be(config.SessionId);
        test.SessionResultId.Should().Be(config.SessionResultId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public async Task Calculate_ShouldGroupTeams_WithMaxNrOfRows(int groupRowCount)
    {
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount).Concat(new[] { default(long?) });
        var rowsPerTeam = 3;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.MaxResultsPerGroup = groupRowCount;
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveCount(teamCount);
        foreach (var teamRow in test.ResultRows)
        {
            teamRow.ScoredMemberResultRowIds.Should().HaveCount(Math.Min(rowsPerTeam, groupRowCount));
            teamRow.ScoredMemberResultRowIds.OrderBy(x => x).Should()
                .BeEquivalentTo(data.ResultRows
                    .Where(x => x.TeamId == teamRow.TeamId)
                    .OrderBy(x => x.FinalPosition)
                    .Take(groupRowCount)
                    .Select(x => x.ScoredResultRowId)
                    .OrderBy(x => x)
                );
        }
    }

    [Fact]
    public async Task Calculate_ShouldAccumulateTeamResultData()
    {
        var groupRowCount = 3;
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 5;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.MaxResultsPerGroup = groupRowCount;
        config.PointRule = CalculationMockHelper.MockPointRule();
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveCount(teamCount);
        foreach (var teamRow in test.ResultRows)
        {
            var memberRows = data.ResultRows
                .Where(x => x.TeamId == teamRow.TeamId)
                .OrderBy(x => x.FinalPosition)
                .Take(groupRowCount);
            teamRow.AvgLapTime.Should().BeCloseTo(TimeSpan.FromSeconds(memberRows.Sum(x => x.AvgLapTime.TotalSeconds * x.CompletedLaps) / memberRows.Sum(x => x.CompletedLaps)), TimeSpan.FromMilliseconds(1));
            teamRow.BonusPoints.Should().Be(0);
            teamRow.CompletedLaps.Should().Be(memberRows.Sum(x => x.CompletedLaps));
            teamRow.FastestLapTime.Should().Be(memberRows.Min(x => x.FastestLapTime));
            teamRow.MemberId.Should().BeNull();
            teamRow.Firstname.Should().BeEmpty();
            teamRow.Lastname.Should().BeEmpty();
            teamRow.Incidents.Should().Be(memberRows.Sum(x => x.Incidents));
            teamRow.Interval.Should().BeCloseTo(TimeSpan.FromSeconds(memberRows.Sum(x => x.Interval.TotalSeconds)), TimeSpan.FromMilliseconds(1));
            teamRow.LeadLaps.Should().Be(memberRows.Sum(x => x.LeadLaps));
            teamRow.PenaltyPoints.Should().Be(memberRows.Sum(x => x.PenaltyPoints));
            teamRow.QualifyingTime.Should().Be(memberRows.Min(x => x.QualifyingTime));
            teamRow.RacePoints.Should().Be(memberRows.Sum(x => x.RacePoints + x.BonusPoints));
        }
    }

    [Fact]
    public async Task Calculate_ShouldCalculateTotalPoints()
    {
        var groupRowCount = 3;
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 5;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule();
        config.MaxResultsPerGroup = groupRowCount;
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        foreach (var row in test.ResultRows)
        {
            row.TotalPoints.Should().Be(row.RacePoints + row.BonusPoints - row.PenaltyPoints);
        }
    }

    [Fact]
    public async Task Calculate_ShouldSortFinal()
    {
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 5;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortFinal: rows => rows.OrderBy(x => x.TotalPoints).ToList());
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().BeInAscendingOrder(x => x.TotalPoints);
    }

    [Fact]
    public async Task Calculate_ShouldSetFinalPosition()
    {
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 5;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedFinalPositions = Enumerable.Range(1, teamCount);
        test.ResultRows.Select(x => x.FinalPosition).Should().BeEquivalentTo(expectedFinalPositions);
    }

    [Theory]
    [InlineData(PenaltyType.Points)]
    [InlineData(PenaltyType.Time)]
    [InlineData(PenaltyType.Position)]
    [InlineData(PenaltyType.Disqualification)]
    public async Task Calculate_ShouldApplyTeamPenalties(PenaltyType penaltyType)
    {
        var groupRowCount = 3;
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 3;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.MaxResultsPerGroup = groupRowCount;
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortForPoints: x => x.OrderBy(x => x.TeamId).ToList());

        var testTeamId = teamIds.OrderBy(x => x).First();
        var testTeamRows = data.ResultRows.Where(x => x.TeamId ==  testTeamId).ToList();
        testTeamRows.ForEach(x => x.PenaltyPoints = 0);
        var penalty = new AddPenaltyCalculationData()
        {
            TeamId = testTeamId,
            Type = penaltyType,
            Points = 3,
            Time = TimeSpan.FromSeconds(3),
            Positions = 1,
        };
        testTeamRows.ForEach(x => x.AddPenalties = new[] { penalty });
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testRow = test.ResultRows.Single(x => x.TeamId == testTeamId);

        switch (penaltyType) 
        {
            case PenaltyType.Points:
                testRow.PenaltyPoints.Should().Be(penalty.Points);
                testRow.TotalPoints.Should().Be(testRow.RacePoints + testRow.BonusPoints - testRow.PenaltyPoints);
                break;
            case PenaltyType.Time:
                var interval = TimeSpan.FromSeconds(testTeamRows.Sum(x => x.Interval.TotalSeconds));
                testRow.Interval.Should().BeCloseTo(interval + penalty.Time, TimeSpan.FromSeconds(0.001));
                break;
            case PenaltyType.Position:
                var position = test.ResultRows.OrderBy(x => x.TeamId).ToList().IndexOf(testRow) + 1;
                testRow.FinalPosition.Should().Be(position + penalty.Positions);
                break;
            case PenaltyType.Disqualification:
                testRow.Status.Should().Be((int)RaceStatus.Disqualified);
                break;
            default:
                break;
        }
    }

    [Fact]
    public async Task Calculate_ShouldApplyReviewPenalty()
    {
        var groupRowCount = 3;
        var teamCount = 3;
        var teamIds = fixture.CreateMany<long?>(teamCount);
        var rowsPerTeam = 3;
        var data = GetCalculationData();
        data.ResultRows = GetTestRows(teamIds, rowsPerTeam);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.MaxResultsPerGroup = groupRowCount;
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortFinal: x => x.OrderBy(x => x.TeamId).ToList());

        var testTeamId = teamIds.OrderBy(x => x).First();
        var testTeamRows = data.ResultRows.Where(x => x.TeamId == testTeamId).ToList();
        testTeamRows.ForEach(x => x.PenaltyPoints = 0);
        var voteData = new AcceptedReviewVoteCalculationData()
        {
            TeamAtFaultId = testTeamId,
            DefaultPenalty = 3,
        };
        data.AcceptedReviewVotes = new[] { voteData };
        
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testRow = test.ResultRows.Single(x => x.TeamId == testTeamId);

        testRow.PenaltyPoints.Should().Be(voteData.DefaultPenalty);
        testRow.TotalPoints.Should().Be(testRow.RacePoints + testRow.BonusPoints - testRow.PenaltyPoints);
    }

    [Fact]
    public async Task Calculate_ShouldNotCrashOnEmptyRows()
    {
        const int rowCount = 0;
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .CreateMany(rowCount);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = async () => await sut.Calculate(data);

        await test.Should().NotThrowAsync();
    }

    private TeamSessionCalculationService CreateSut()
    {
        return fixture.Create<TeamSessionCalculationService>();
    }

    private SessionCalculationData GetCalculationData()
    {
        return fixture.Create<SessionCalculationData>();
    }

    private SessionCalculationConfiguration GetCalculationConfiguration(long leagueId, long? sessionId)
    {
        return fixture
            .Build<SessionCalculationConfiguration>()
            .With(x => x.LeagueId, leagueId)
            .With(x => x.SessionId, sessionId)
            .With(x => x.ResultKind, ResultKind.Team)
            .Create();
    }

    private IPostprocessComposer<ResultRowCalculationData> TestRowBuilder()
    {
        return fixture.Build<ResultRowCalculationData>()
            .Without(x => x.AddPenalties);
    }

    private IPostprocessComposer<ResultRowCalculationData> TeamTestRowBuilder(IEnumerable<long?> teamIds)
    {
        var idSequence = teamIds.CreateSequence();
        return TestRowBuilder()
            .With(x => x.TeamId, () => idSequence());
    }

    private IEnumerable<ResultRowCalculationData> GetTestRows(IEnumerable<long?> teamIds, int rowsPerTeam)
    {
        return TeamTestRowBuilder(teamIds)
            .CreateMany(teamIds.Count() * rowsPerTeam);
    }
}
