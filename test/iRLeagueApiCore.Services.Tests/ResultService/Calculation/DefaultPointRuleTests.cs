using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class DefaultPointRuleTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public void SortForPoints_ShouldNotChangeOrder()
    {
        var rows = GetTestRows(fixture);
        var sut = GetPointRule(fixture);

        var test = sut.SortForPoints(rows.ToList());

        test.Should().BeEquivalentTo(rows);
    }

    [Fact]
    public void ApplyPoints_ShouldCalculateTotalPoints()
    {
        var rows = GetTestRows(fixture);
        var sut = GetPointRule(fixture);

        var test = sut.ApplyPoints(fixture.Create<SessionCalculationData>(), rows.ToList());

        foreach (var row in test)
        {
            row.TotalPoints.Should().Be(row.RacePoints + row.BonusPoints - row.PenaltyPoints);
        }
    }

    private static IEnumerable<ResultRowCalculationData> GetTestRows(Fixture fixture)
    {
        return fixture
            .Build<ResultRowCalculationData>()
            .Without(x => x.AddPenalties)
            .CreateMany(10).ToList();
    }

    private static DefaultPointRule<ResultRowCalculationData> GetPointRule(Fixture fixture)
    {
        return fixture.Create<DefaultPointRule<ResultRowCalculationData>>();
    }
}
