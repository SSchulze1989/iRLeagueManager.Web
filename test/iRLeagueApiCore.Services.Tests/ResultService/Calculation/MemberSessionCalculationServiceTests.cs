using AutoFixture.Dsl;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueApiCore.Mocking.Extensions;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class MemberSessionCalculationServiceTests
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

    [Fact]
    public async Task Calculate_ShouldApplyPoints_BasedOnOriginalPosition()
    {
        var data = GetCalculationData();
        data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(getRacePoints: (row, pos) => pos);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveSameCount(data.ResultRows);
        foreach ((var row, var pos) in test.ResultRows.Select((x, i) => (x, i + 1)))
        {
            row.RacePoints.Should().Be(pos);
            row.TotalPoints.Should().Be(row.RacePoints);
        }
    }

    [Fact]
    public async Task Calculate_ShouldApplyPoints_BasedOnSortedPosition()
    {
        var data = GetCalculationData();
        data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortForPoints: rows => rows.OrderBy(x => x.FinishPosition).ToList(),
            getRacePoints: (row, pos) => pos);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveSameCount(data.ResultRows);
        test.ResultRows.Select(x => x.FinishPosition).Should()
            .BeEquivalentTo(data.ResultRows.Select(x => x.FinishPosition));
        foreach ((var row, var pos) in test.ResultRows.OrderBy(x => x.FinishPosition).Select((x, i) => (x, i + 1)))
        {
            row.RacePoints.Should().Be(pos);
        }
    }

    [Fact]
    public async Task Calculate_ShouldSortFinal()
    {
        var data = GetCalculationData();
        data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortFinal: rows => rows.OrderBy(x => x.FinishPosition).ToList());
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().BeInAscendingOrder(x => x.FinishPosition);
    }

    [Fact]
    public async Task Calculate_ShouldSetFinalPositionAndChange()
    {
        var data = GetCalculationData();
        data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedFinalPositions = Enumerable.Range(1, data.ResultRows.Count());
        var expectedFinalPositionChanges = data.ResultRows.Select((x, i) => (int)(x.StartPosition - (i + 1)));
        test.ResultRows.Select(x => x.FinalPosition).Should().BeEquivalentTo(expectedFinalPositions);
        test.ResultRows.Select(x => x.FinalPositionChange).Should().BeEquivalentTo(expectedFinalPositionChanges);
    }

    [Fact]
    public async Task Calculate_ShouldSetFastestLap()
    {
        var data = GetCalculationData();
        var rows = data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedLapRow = rows.MinBy(x => x.FastestLapTime)!;
        test.FastestLap.Should().Be(expectedLapRow.FastestLapTime);
        test.FastestLapDriverMemberId.Should().Be(expectedLapRow.MemberId);
    }

    [Fact]
    public async Task Calculate_ShouldSetFastestAvgLap()
    {
        var data = GetCalculationData();
        var rows = data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedLapRow = rows.MinBy(x => x.AvgLapTime)!;
        test.FastestAvgLap.Should().Be(expectedLapRow.AvgLapTime);
        test.FastestAvgLapDriverMemberId.Should().Be(expectedLapRow.MemberId);
    }

    [Fact]
    public async Task Calculate_ShouldSetFastestQualyLap()
    {
        var data = GetCalculationData();
        var rows = data.ResultRows = GetTestRows();
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedLapRow = rows.MinBy(x => x.QualifyingTime)!;
        test.FastestQualyLap.Should().Be(expectedLapRow.QualifyingTime);
        test.FastestQualyLapDriverMemberId.Should().Be(expectedLapRow.MemberId);
    }

    [Fact]
    public async Task Calculate_ShouldSetHardChargers()
    {
        const int rowCount = 3;
        var startPositions = new[] { 3, 2, 5 }.AsEnumerable().GetEnumerator();
        var finishPositions = new[] { 1, 2, 3 }.AsEnumerable().GetEnumerator();
        var data = GetCalculationData();
        var rows = data.ResultRows = TestRowBuilder()
            .With(x => x.StartPosition, () => startPositions.Next())
            .With(x => x.FinishPosition, () => finishPositions.Next())
            .CreateMany(rowCount);
        var pointRule = CalculationMockHelper.MockPointRule(
            sortFinal: rows => rows.OrderBy(x => x.FinishPosition).ToList());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedHardChargers = new[] { rows.ElementAt(0), rows.ElementAt(2) }.Select(x => x.MemberId);
        test.HardChargers.Should().BeEquivalentTo(expectedHardChargers);
    }

    [Fact]
    public async Task Calculate_ShouldSetCleanestDrivers()
    {
        const int rowCount = 3;
        var incidents = new[] { 1, 2, 1 }.AsEnumerable().GetEnumerator();
        var data = GetCalculationData();
        var rows = data.ResultRows = TestRowBuilder()
            .With(x => x.Incidents, () => incidents.Next())
            .CreateMany(rowCount);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expectedCleanestDrivers = new[] { rows.ElementAt(0), rows.ElementAt(2) }.Select(x => x.MemberId);
        test.CleanestDrivers.Should().BeEquivalentTo(expectedCleanestDrivers);
    }

    [Fact]
    public async Task Calculate_ShoulNotThrow_WhenColumnsHaveDefaultValues()
    {
        const int rowCount = 3;
        var data = GetCalculationData();
        var rows = data.ResultRows = TestRowBuilder()
            .OmitAutoProperties()
            .With(x => x.MemberId)
            .With(x => x.Firstname)
            .With(x => x.Lastname)
            .CreateMany(rowCount);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = async () => await sut.Calculate(data);

        await test.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Calculate_ShouldApplyTimePenaltyToInterval()
    {
        const int rowCount = 3;
        const int spread = 5;
        // create fixed intervals to test sorting
        var intervals = Enumerable.Range(0, rowCount)
            .Select(x => TimeSpan.FromSeconds(x * spread))
            .CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.Interval, () => intervals())
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Time)
            .With(x => x.Time, TimeSpan.FromSeconds(spread + 1))
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(0);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        testResultRow.Interval.Should().Be(penaltyRow.Interval + addPenalty.Time);
    }

    [Fact]
    public async Task Calculate_ShouldSortAfterApplyingTimePenalty()
    {
        const int rowCount = 3;
        const int spread = 5;
        const int penaltyIndex = 1;
        // create fixed intervals to test sorting
        var intervals = Enumerable.Range(0, rowCount)
            .Select(x => TimeSpan.FromSeconds(x * spread))
            .CreateSequence();
        var positions = Enumerable.Range(1, rowCount).CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.Interval, () => intervals())
            .With(x => x.FinishPosition, () => positions())
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Time)
            .With(x => x.Time, TimeSpan.FromSeconds(spread + 1))
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(penaltyIndex);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortForPoints: x => x.OrderBy(x => x.Interval).ToList());
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        test.ResultRows.ToList().IndexOf(testResultRow).Should().Be(penaltyIndex + 1);
        testResultRow.FinalPosition.Should().Be((int)penaltyRow.FinishPosition + 1);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    [InlineData(3, 3)]
    public async Task Calculate_ShouldApplyPositioningPenalty(int penaltyIndex, int positionPenalty)
    {
        const int rowCount = 5;
        // create fixed intervals to test sorting
        var positions = Enumerable.Range(1, rowCount).CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, () => positions())
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Position)
            .With(x => x.Positions, positionPenalty)
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(penaltyIndex);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var expIndex = Math.Min(penaltyIndex + positionPenalty, rowCount - 1);
        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        test.ResultRows.ToList().IndexOf(testResultRow).Should().Be(expIndex);
        testResultRow.FinalPosition.Should().Be(expIndex + 1);
    }

    [Theory]
    [InlineData(0, 2, 2, 2, 1, 3)]
    [InlineData(0, 2, 2, 1, 1, 1)]
    [InlineData(1, 1, 2, 2, 1, 3)]
    public async Task Calculate_ShouldApplyPositioningPenalty_WithMultiple(
        int penaltyIndex1, int positionPenalty1, int expIndex1,
        int penaltyIndex2, int positionPenalty2, int expIndex2)
    {
        const int rowCount = 5;
        // create fixed intervals to test sorting
        var positions = Enumerable.Range(1, rowCount)
            .CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, () => positions())
            .CreateMany(rowCount);
        var addPenalty1 = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Position)
            .With(x => x.Positions, positionPenalty1)
            .Create();
        var addPenalty2 = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Position)
            .With(x => x.Positions, positionPenalty2)
            .Create();
        var penaltyRow1 = data.ResultRows.ElementAt(penaltyIndex1);
        penaltyRow1.AddPenalties = new[] { addPenalty1 };
        var penaltyRow2 = data.ResultRows.ElementAt(penaltyIndex2);
        penaltyRow2.AddPenalties = new[] { addPenalty2 };
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow1 = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow1.ScoredResultRowId);
        var testResultRow2 = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow2.ScoredResultRowId);
        test.ResultRows.ToList().IndexOf(testResultRow1).Should().Be(expIndex1);
        testResultRow1.FinalPosition.Should().Be(expIndex1 + 1);
        test.ResultRows.ToList().IndexOf(testResultRow2).Should().Be(expIndex2);
        testResultRow2.FinalPosition.Should().Be(expIndex2 + 1);
    }

    [Fact]
    public async Task Calculate_ShouldApplyPointsPenalty()
    {
        const int rowCount = 3;
        const int spread = 5;
        const int penaltyIndex = 1;
        // create fixed intervals to test sorting
        var points = Enumerable.Range(0, rowCount)
            .Select(x => (rowCount - x) * spread)
            .CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.RacePoints, () => points())
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Points)
            .With(x => x.Points, spread + 1)
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(penaltyIndex);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortFinal: x => x.OrderByDescending(x => x.TotalPoints).ToList());
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        var expIndex = penaltyIndex + 1;
        testResultRow.PenaltyPoints.Should().Be(addPenalty.Points);
        testResultRow.TotalPoints.Should().Be(testResultRow.RacePoints - testResultRow.PenaltyPoints);
        test.ResultRows.ToList().IndexOf(testResultRow).Should().Be(expIndex);
    }

    [Fact]
    public async Task Calculate_ShouldApplyNegativePointsPenaltyAsBonus()
    {
        const int rowCount = 3;
        const int spread = 5;
        const int penaltyIndex = 1;
        // create fixed intervals to test sorting
        var points = Enumerable.Range(0, rowCount)
            .Select(x => (rowCount - x) * spread)
            .CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.RacePoints, () => points())
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Points)
            .With(x => x.Points, -(spread + 1))
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(penaltyIndex);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            sortFinal: x => x.OrderByDescending(x => x.TotalPoints).ToList());
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        var expIndex = penaltyIndex - 1;
        testResultRow.BonusPoints.Should().Be(-addPenalty.Points);
        testResultRow.TotalPoints.Should().Be(testResultRow.RacePoints + testResultRow.BonusPoints);
        test.ResultRows.ToList().IndexOf(testResultRow).Should().Be(expIndex);
    }

    [Fact]
    public async Task Calculate_ShouldApplyPositionBonusPoints()
    {
        var startPositions = new double[] { 3, 2, 1 }.CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.StartPosition, startPositions)
            .CreateMany(3);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        config.PointRule = CalculationMockHelper.MockPointRule(
            bonusPoints: new BonusPointConfiguration[]
            {
                new() { Type = BonusPointType.QualyPosition, Value = 1, Points = 2 },
                new() { Type = BonusPointType.QualyPosition, Value = 2, Points = 1 },
            });
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveSameCount(data.ResultRows);
        var testRow1 = test.ResultRows.ElementAt(2);
        var testRow2 = test.ResultRows.ElementAt(1);
        testRow1.BonusPoints.Should().Be(2);
        testRow2.BonusPoints.Should().Be(1);
    }

    [Fact]
    public async Task Calculate_ShouldApplyCustomBonusPoints()
    {
        var startPositions = new double[] { 3, 2, 1 }.CreateSequence();
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.StartPosition, startPositions)
            .With(x => x.LeadLaps, 0)
            .CreateMany(3);
        data.ResultRows.First().LeadLaps = 16;
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> condition1 = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.StartPosition), ComparatorType.IsEqual, new[] { "1" },
                MatchedValueAction.Keep, allowForEach: true);
        RowFilter<ResultRowCalculationResult> condition2 = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.StartPosition), ComparatorType.IsEqual, new[] { "2" },
                MatchedValueAction.Keep, allowForEach: true);
        RowFilter<ResultRowCalculationResult> condition3 = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.LeadLaps), ComparatorType.ForEach, new[] { "5" },
                MatchedValueAction.Keep, allowForEach: true);
        config.PointRule = CalculationMockHelper.MockPointRule(
            bonusPoints: new BonusPointConfiguration[]
            {
                new() { Type = BonusPointType.Custom, Points = 2, Conditions = new(new[] { (FilterCombination.And, condition1) }) },
                new() { Type = BonusPointType.Custom, Points = 1, Conditions = new(new[] { (FilterCombination.And, condition2) }) },
                new() { Type = BonusPointType.Custom, Points = 2, Conditions = new(new[] { (FilterCombination.And, condition3) }) }, 
            });
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveSameCount(data.ResultRows);
        var testRow1 = test.ResultRows.ElementAt(2);
        var testRow2 = test.ResultRows.ElementAt(1);
        var testRow3 = test.ResultRows.ElementAt(0);
        testRow1.BonusPoints.Should().Be(2);
        testRow2.BonusPoints.Should().Be(1);
        testRow3.BonusPoints.Should().Be(6);
    }

    [Fact]
    public async Task Calculate_ShouldAddAutoPenalties()
    {
        var incidents = new[] { 2.0, 4.0, 8.0 }; 
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.Incidents, incidents.CreateSequence())
            .CreateMany(incidents.Length);
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.Incidents),
            ComparatorType.ForEach,
            new[] { "4.0" },
            MatchedValueAction.Keep,
            allowForEach: true);
        var autoPenalty = new AutoPenaltyConfigurationData()
        {
            Conditions = new FilterGroupRowFilter<ResultRowCalculationResult>(new[] { (FilterCombination.And, filter) }),
            Description = "Test Autopenalty",
            Points = 10,
            Type = PenaltyType.Points,
        };
        config.PointRule = CalculationMockHelper.MockPointRule(
            autoPenalties: new[] { autoPenalty });
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testRow1 = test.ResultRows.ElementAt(1);
        testRow1.AddPenalties.Should().HaveCount(1);
        testRow1.AddPenalties.First().Points.Should().Be(autoPenalty.Points);
        testRow1.PenaltyPoints.Should().Be(autoPenalty.Points);
        var testRow2 = test.ResultRows.ElementAt(2);
        testRow2.AddPenalties.Should().HaveCount(1);
        testRow2.AddPenalties.First().Points.Should().Be(autoPenalty.Points*2);
        testRow2.PenaltyPoints.Should().Be(autoPenalty.Points*2);
    }

    [Fact]
    public async Task Calculate_ShouldApplyResultFilters()
    {
        var pos = Enumerable.Range(1, 5).Select(x => (double)x);
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, pos.CreateSequence())
            .CreateMany(pos.Count());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.FinishPosition),
            ComparatorType.IsSmallerOrEqual,
            new[] { "3" },
            MatchedValueAction.Keep);
        config.PointRule = CalculationMockHelper.MockPointRule(
            resultFilters: new(new[] { (FilterCombination.And, filter) }));
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveCount(3);
    }

    [Fact]
    public async Task Calculate_ShouldApplyChampSeasonFilters()
    {
        var pos = Enumerable.Range(1, 5).Select(x => (double)x);
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, pos.CreateSequence())
            .CreateMany(pos.Count());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.FinishPosition),
            ComparatorType.IsSmallerOrEqual,
            new[] { "3" },
            MatchedValueAction.Keep);
        config.PointRule = CalculationMockHelper.MockPointRule(
            champSeasonFilters: new(new[] { (FilterCombination.And, filter) }));
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Should().HaveCount(3);
    }

    [Fact]
    public async Task Calculate_ShouldApplyPointFilters()
    {
        var pos = Enumerable.Range(1, 5).Select(x => (double)x);
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, pos.CreateSequence())
            .CreateMany(pos.Count());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.FinishPosition),
            ComparatorType.IsSmallerOrEqual,
            new[] { "3" },
            MatchedValueAction.Keep);
        config.PointRule = CalculationMockHelper.MockPointRule(
            pointFilters: new(new[] { (FilterCombination.And, filter) }),
            getRacePoints: (x, p) => 1);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Take(3).Should().Match(x => x.All(r => r.RacePoints == 1));
        test.ResultRows.Skip(3).Should().Match(x => x.None(r => r.RacePoints == 1));
    }

    [Fact]
    public async Task Calculate_ShouldSetPointsZero_WhenFiltered()
    {
        var pos = Enumerable.Range(1, 5).Select(x => (double)x);
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, pos.CreateSequence())
            .With(x => x.RacePoints, 2)
            .CreateMany(pos.Count());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.FinishPosition),
            ComparatorType.IsSmallerOrEqual,
            new[] { "3" },
            MatchedValueAction.Keep);
        config.PointRule = CalculationMockHelper.MockPointRule(
            pointFilters: new(new[] { (FilterCombination.And, filter) }),
            getRacePoints: (x, p) => 1);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Take(3).Should().Match(x => x.All(r => r.RacePoints == 1));
        test.ResultRows.Skip(3).Should().Match(x => x.All(r => r.RacePoints == 0));
    }

    [Fact]
    public async Task Calculate_ShouldSetPointsEligible()
    {
        var pos = Enumerable.Range(1, 5).Select(x => (double)x);
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.FinishPosition, pos.CreateSequence())
            .With(x => x.RacePoints, 2)
            .CreateMany(pos.Count());
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        RowFilter<ResultRowCalculationResult> filter = new ColumnValueRowFilter(
            nameof(ResultRowCalculationResult.FinishPosition),
            ComparatorType.IsSmallerOrEqual,
            new[] { "3" },
            MatchedValueAction.Keep);
        config.PointRule = CalculationMockHelper.MockPointRule(
            pointFilters: new(new[] { (FilterCombination.And, filter) }),
            getRacePoints: (x, p) => 1);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        test.ResultRows.Take(3).Should().Match(x => x.All(r => r.PointsEligible == true));
        test.ResultRows.Skip(3).Should().Match(x => x.All(r => r.PointsEligible == false));
    }

    [Fact]
    public async Task Calculate_ShouldApplyDsqPenalty()
    {
        const int rowCount = 3;
        var data = GetCalculationData();
        data.ResultRows = TestRowBuilder()
            .With(x => x.Status, (int)RaceStatus.Running)
            .CreateMany(rowCount);
        var addPenalty = fixture.Build<AddPenaltyCalculationData>()
            .With(x => x.Type, PenaltyType.Disqualification)
            .Create();
        var penaltyRow = data.ResultRows.ElementAt(0);
        penaltyRow.AddPenalties = new[] { addPenalty };
        var config = GetCalculationConfiguration(data.LeagueId, data.SessionId);
        fixture.Register(() => config);
        var sut = CreateSut();

        var test = await sut.Calculate(data);

        var testResultRow = test.ResultRows.First(x => x.ScoredResultRowId == penaltyRow.ScoredResultRowId);
        testResultRow.Status.Should().Be((int)RaceStatus.Disqualified);
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

    private MemberSessionCalculationService CreateSut()
    {
        return fixture.Create<MemberSessionCalculationService>();
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
            .With(x => x.IsCombinedResult, false)
            .Create();
    }

    private IPostprocessComposer<ResultRowCalculationData> TestRowBuilder()
    {
        return fixture.Build<ResultRowCalculationData>()
            .Without(x => x.RacePoints)
            .Without(x => x.BonusPoints)
            .Without(x => x.PenaltyPoints)
            .Without(x => x.AddPenalties);
    }

    private IEnumerable<ResultRowCalculationData> GetTestRows()
    {
        return TestRowBuilder().CreateMany();
    }
}
