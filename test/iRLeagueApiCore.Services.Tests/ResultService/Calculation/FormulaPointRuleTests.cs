using AutoFixture;
using AutoFixture.Dsl;
using iRLeagueApiCore.Mocking.Extensions;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;
using System.Xml.Serialization;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;
public sealed class FormulaPointRuleTests
{
    private readonly Fixture fixture = new();

    [Theory]
    [InlineData("pos", 1)]
    [InlineData("position", 1)]
    [InlineData("start", 2)]
    [InlineData("start_position", 2)]
    [InlineData("count", 1)]
    [InlineData("driver_count", 1)]
    [InlineData("irating", 4200)]
    [InlineData("flap", 4.20)]
    [InlineData("fastest_lap", 4.20)]
    [InlineData("qlap", 42.0)]
    [InlineData("qualy_lap", 42.0)]
    [InlineData("avglap", 420.0)]
    [InlineData("avg_lap", 420.0)]
    [InlineData("sof", 1234)]
    [InlineData("strength_of_field", 1234)]
    [InlineData("flapsession", 1.23)]
    [InlineData("session_fastest_lap", 1.23)]
    [InlineData("qlapsession", 12.3)]
    [InlineData("session_fastest_qualy_lap", 12.3)]
    [InlineData("avglapsession", 123.0)]
    [InlineData("session_fastest_avg_lap", 123.0)]
    public void ApplyPoints_ShouldWorkWithAliases(string alias, double expected)
    {
        var formula = $"[{alias}]";
        var rows = TestRowsBuilder(fixture, 1)
            .With(x => x.OldIrating, 4200)
            .With(x => x.FastestLapTime, TimeSpan.FromSeconds(4.20))
            .With(x => x.QualifyingTime, TimeSpan.FromSeconds(42.0))
            .With(x => x.AvgLapTime, TimeSpan.FromSeconds(420))
            .With(x => x.StartPosition, 2)
            .CreateMany(1);
        var session = SessionDataBuilder(fixture, rows)
            .With(x => x.Sof, 1234)
            .With(x => x.FastestLap, TimeSpan.FromSeconds(1.23))
            .With(x => x.FastestQualyLap, TimeSpan.FromSeconds(12.3))
            .With(x => x.FastestAvgLap, TimeSpan.FromSeconds(123.0))
            .Create();
        var sut = GetPointRule(formula, allowNegativePoints: false);

        var test = sut.ApplyPoints(session, rows.ToList()).ToList();

        test.First().RacePoints.Should().Be(expected);
    }

    [Theory]
    [InlineData("10 - ([pos] - 1)", new double[] { 10, 9, 8 }, false)]
    [InlineData("1 - ([pos] - 1)", new double[] { 1, 0, 0 }, false)]
    [InlineData("1 - ([pos] - 1)", new double[] { 1, 0, -1 }, true)]
    public void Should_ApplyPointsBasedOnPosition(string formula, double[] expected, bool allowNegativePoints)
    {
        var rows = GetTestRows(fixture, expected.Length);
        var session = GetSessionData(fixture, rows);
        var sut = GetPointRule(formula, allowNegativePoints);

        var test = sut.ApplyPoints(session, rows.ToList()).ToList();

        foreach (var (row, points) in test.Zip(expected))
        {
            row.RacePoints.Should().Be(points);
        }
    }

    [Theory]
    [InlineData("[count]", new double[] { 5, 5, 5, 5, 5 }, false)]
    [InlineData("[count] - ([pos] - 1)", new double[] { 3, 2, 1 }, false)]
    [InlineData("[count] - 2 - ([pos] - 1)", new double[] { 1, 0, 0 }, false)]
    [InlineData("[count] - 2 - ([pos] - 1)", new double[] { 1, 0, -1 }, true)]
    [InlineData("[count] * 10 - ([pos] - 1)*[count]", new double[] { 30, 27, 24 }, false)]
    public void Should_ApplyPointsBasedOnCount(string formula, double[] expected, bool allowNegativePoints)
    {
        var rows = GetTestRows(fixture, expected.Length);
        var session = GetSessionData(fixture, rows);
        var sut = GetPointRule(formula, allowNegativePoints);

        var test = sut.ApplyPoints(session, rows.ToList()).ToList();

        foreach (var (row, points) in test.Zip(expected))
        {
            row.RacePoints.Should().Be(points);
        }
    }

    private static IPostprocessComposer<ResultRowCalculationResult> TestRowsBuilder(Fixture fixture, int count)
    {
        var position = Enumerable.Range(1, count).AsEnumerable().CreateSequence();
        return fixture
            .Build<ResultRowCalculationResult>()
            .With(x => x.FinishPosition, () => position())
            .Without(x => x.AddPenalties);
    }

    private static IEnumerable<ResultRowCalculationResult> GetTestRows(Fixture fixture, int count)
    {
        return TestRowsBuilder(fixture, count).CreateMany(count).ToList();
    }

    private static IPostprocessComposer<SessionCalculationData> SessionDataBuilder(Fixture fixture, IEnumerable<ResultRowCalculationResult> rows)
    {
        return fixture.Build<SessionCalculationData>()
            .With(x => x.ResultRows, rows);
    }

    private static SessionCalculationData GetSessionData(Fixture fixture, IEnumerable<ResultRowCalculationResult> rows)
    {
        return SessionDataBuilder(fixture, rows).Create();
    }

    private static FormulaPointRule GetPointRule(string formula, bool allowNegativePoints)
    {
        return new(formula, allowNegativePoints);
    }
}
