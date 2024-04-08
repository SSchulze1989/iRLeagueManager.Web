using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueApiCore.Mocking.Extensions;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class CalculationPointRuleBaseTests
{
    private readonly Fixture fixture;

    public CalculationPointRuleBaseTests()
    {
        this.fixture = new();
    }

    [Fact]
    public void SortForPoints_ShouldSort_WithSingleOption()
    {
        var pos = 1;
        var sequence = (new[] { 2, 3, 1, 5, 4 }).AsEnumerable().GetEnumerator();
        var rows = fixture.Build<ResultRowCalculationResult>()
            .With(x => x.FinishPosition, () => pos++)
            .With(x => x.RacePoints, () => sequence.Next())
            .CreateMany(5);
        var sut = CreateSut();
        sut.PointSortOptions = new[] { SortOptions.RacePtsAsc };

        var test = sut.SortForPoints(rows);

        var expected = new[] { 3, 1, 2, 5, 4 };
        foreach ((var testRow, var expectedPos) in test.Zip(expected))
        {
            testRow.FinishPosition.Should().Be(expectedPos);
        }
    }

    [Fact]
    public void SortForPoints_ShouldSort_WithMultipleOption()
    {
        var pos = 1;
        var sequence1 = (new[] { 2, 2, 1, 4, 4 }).AsEnumerable().GetEnumerator();
        var sequence2 = (new[] { 5, 4, 3, 2, 2 }).AsEnumerable().GetEnumerator();
        var sequence3 = (new[] { 1, 2, 3, 5, 4 }).AsEnumerable().GetEnumerator();
        var rows = fixture.Build<ResultRowCalculationResult>()
            .With(x => x.FinishPosition, () => pos++)
            .With(x => x.RacePoints, () => sequence1.Next())
            .With(x => x.BonusPoints, () => sequence2.Next())
            .With(x => x.Incidents, () => sequence3.Next())
            .CreateMany(5);
        var sut = CreateSut();
        sut.PointSortOptions = new[] { SortOptions.RacePtsAsc, SortOptions.BonusPtsAsc, SortOptions.IncsAsc };

        var test = sut.SortForPoints(rows);

        var expected = new[] { 3, 2, 1, 5, 4 };
        foreach ((var testRow, var expectedPos) in test.Zip(expected))
        {
            testRow.FinishPosition.Should().Be(expectedPos);
        }
    }

    [Fact]
    public void SortFinal_ShouldSort_WithSingleOption()
    {
        var pos = 1;
        var sequence = (new[] { 2, 3, 1, 5, 4 }).AsEnumerable().GetEnumerator();
        var rows = fixture.Build<ResultRowCalculationResult>()
            .With(x => x.FinishPosition, () => pos++)
            .With(x => x.RacePoints, () => sequence.Next())
            .CreateMany(5);
        var sut = CreateSut();
        sut.FinalSortOptions = new[] { SortOptions.RacePtsAsc };

        var test = sut.SortFinal(rows);

        var expected = new[] { 3, 1, 2, 5, 4 };
        foreach ((var testRow, var expectedPos) in test.Zip(expected))
        {
            testRow.FinishPosition.Should().Be(expectedPos);
        }
    }

    [Fact]
    public void SortFinal_ShouldSort_WithMultipleOption()
    {
        var pos = 1;
        var sequence1 = (new[] { 2, 2, 1, 4, 4 }).AsEnumerable().GetEnumerator();
        var sequence2 = (new[] { 5, 4, 3, 2, 2 }).AsEnumerable().GetEnumerator();
        var sequence3 = (new[] { 1, 2, 3, 5, 4 }).AsEnumerable().GetEnumerator();
        var rows = fixture.Build<ResultRowCalculationResult>()
            .With(x => x.FinishPosition, () => pos++)
            .With(x => x.RacePoints, () => sequence1.Next())
            .With(x => x.BonusPoints, () => sequence2.Next())
            .With(x => x.Incidents, () => sequence3.Next())
            .CreateMany(5);
        var sut = CreateSut();
        sut.FinalSortOptions = new[] { SortOptions.RacePtsAsc, SortOptions.BonusPtsAsc, SortOptions.IncsAsc };

        var test = sut.SortFinal(rows);

        var expected = new[] { 3, 2, 1, 5, 4 };
        foreach ((var testRow, var expectedPos) in test.Zip(expected))
        {
            testRow.FinishPosition.Should().Be(expectedPos);
        }
    }

    private CalculationPointRuleBase CreateSut()
    {
        var mockSut = new Mock<CalculationPointRuleBase>();
        mockSut.CallBase = true;
        return mockSut.Object;
    }
}
