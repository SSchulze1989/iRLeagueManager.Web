using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Mocking.Extensions;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using System.Globalization;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

public sealed class ColumnValueRowFilterTests
{
    private static readonly Fixture fixture = new();

    private static IEnumerable<object[]> TestValidConstructorData() => new[]
        {
            new object[] { nameof(ResultRowCalculationResult.FinalPosition), new[] { fixture.Create<int>().ToString() } },
            new object[] { nameof(ResultRowCalculationResult.FinishPosition), new[] { fixture.Create<double>().ToString(CultureInfo.InvariantCulture) }},
            new object[] { nameof(ResultRowCalculationResult.Firstname), new[] { fixture.Create<string>() }},
            new object[] { nameof(ResultRowCalculationResult.QualifyingTime), new[] { fixture.Create<TimeSpan>().ToString() }},
        };

    private static IEnumerable<object[]> TestInValidConstructorData() => new[]
        {
            new object[] { nameof(ResultRowCalculationResult.FinalPosition), new[] { fixture.Create<string>().ToString() } },
            new object[] { nameof(ResultRowCalculationResult.FinishPosition), new[] { fixture.Create<string>().ToString(CultureInfo.InvariantCulture) }},
            new object[] { nameof(ResultRowCalculationResult.QualifyingTime), new[] { fixture.Create<string>().ToString() }},
            new object[] { fixture.Create<string>(), new[] { fixture.Create<int>().ToString() }},
        };

    private static IEnumerable<object[]> TestFilterRowsData()
    {
        var matchRow = fixture.Create<ResultRowCalculationResult>();
        return new[]
        {
            new object[] { nameof(ResultRowCalculationResult.CompletedLaps), matchRow, new[] { matchRow.CompletedLaps.ToString() } },
            new object[] { nameof(ResultRowCalculationResult.FastestLapTime), matchRow, new[] { matchRow.FastestLapTime.ToString() } },
            new object[] { nameof(ResultRowCalculationResult.Firstname), matchRow, new[] { matchRow.Firstname.ToString() } },
            new object[] { nameof(ResultRowCalculationResult.CompletedPct), matchRow, new[] { matchRow.CompletedPct.ToString() } },
        };
    }

    [Theory]
    [MemberData(nameof(TestValidConstructorData))]
    public void Constructor_ShouldNotThrow_WithValidValues(string propertyName, IEnumerable<string> filterValues)
    {
        var test = () => CreateSut(propertyName, filterValues);
        test.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(TestInValidConstructorData))]
    public void Constructor_ShouldThrow_WithInvalidValues(string propertyName, IEnumerable<string> filterValues)
    {
        var test = () => CreateSut(propertyName, filterValues);
        test.Should().Throw<ArgumentException>();
    }

    [Theory]
    [MemberData(nameof(TestFilterRowsData))]
    public void FilterRows_ShouldKeepMatchingRows_WhenComparaterIsEqualAndActionIsKeep(string propertyName,
        object matchRow, IEnumerable<string> filterValues)
    {
        var rows = fixture.CreateMany<ResultRowCalculationResult>().ToList();
        rows.Add((ResultRowCalculationResult)matchRow);
        rows = rows.Shuffle().ToList();
        var sut = CreateSut(propertyName, filterValues, ComparatorType.IsEqual, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain((ResultRowCalculationResult)matchRow);
        test.Should().HaveCount(1);
    }

    [Theory]
    [MemberData(nameof(TestFilterRowsData))]
    public void FilterRows_ShouldRemoveMatchingRows_WhenComparaterIsEqualAndActionIsRemove(string propertyName,
        object matchRow, IEnumerable<string> filterValues)
    {
        var rows = fixture.CreateMany<ResultRowCalculationResult>().ToList();
        rows.Add((ResultRowCalculationResult)matchRow);
        rows = rows.Shuffle().ToList();
        var sut = CreateSut(propertyName, filterValues, ComparatorType.IsEqual, MatchedValueAction.Remove);

        var test = sut.FilterRows(rows);

        test.Should().NotContain((ResultRowCalculationResult)matchRow);
        test.Should().HaveCount(rows.Count - 1);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsSmaller()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.IsSmaller, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain(rows.ElementAt(0));
        test.Should().HaveCount(1);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsSmallerOrEqual()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.IsSmallerOrEqual, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain(rows.ElementAt(0));
        test.Should().Contain(cutoffRow);
        test.Should().HaveCount(2);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsEqual()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.IsEqual, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain(cutoffRow);
        test.Should().HaveCount(1);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsBiggerOrEqual()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.IsBiggerOrEqual, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain(cutoffRow);
        test.Should().HaveCount(rows.Count() - 1);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsBigger()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.IsBigger, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().NotContain(rows.ElementAt(0));
        test.Should().NotContain(cutoffRow);
        test.Should().HaveCount(rows.Count() - 2);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsNotEqual()
    {
        var propertyName = nameof(ResultRowCalculationResult.TotalPoints);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.TotalPoints);
        var cutoffRow = rows.ElementAt(1);
        var sut = CreateSut(propertyName, new[] { cutoffRow.TotalPoints.ToString() }, ComparatorType.NotEqual, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().NotContain(cutoffRow);
        test.Should().HaveCount(rows.Count() - 1);
    }

    [Fact]
    public void FilterRows_ShouldFilterValues_WhenComparaterIsInList()
    {
        var propertyName = nameof(ResultRowCalculationResult.Firstname);
        var rows = fixture.CreateMany<ResultRowCalculationResult>(5)
            .OrderBy(x => x.Firstname);
        var testRow1 = rows.ElementAt(1);
        var testRow2 = rows.ElementAt(3);
        var sut = CreateSut(propertyName, new[] { testRow1.Firstname, testRow2.Firstname }, ComparatorType.InList, MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().Contain(testRow1);
        test.Should().Contain(testRow2);
        test.Should().HaveCount(2);
    }

    [Fact]
    public void FilterRows_ShouldMultiplyRows_WhenComparatorIsForEach()
    {
        var propertyName = nameof(ResultRowCalculationResult.Incidents);
        var incidents = new[] { 2.0, 4.0, 8.0 };
        var rows = fixture.Build<ResultRowCalculationResult>()
            .With(x => x.Incidents, incidents.CreateSequence())
            .CreateMany(incidents.Length);
        var testRow1 = rows.ElementAt(0);
        var testRow2 = rows.ElementAt(1);
        var testRow3 = rows.ElementAt(2);
        var sut = CreateSut(propertyName, new[] { "4.0" }, ComparatorType.ForEach, MatchedValueAction.Keep, allowForEach: true);

        var test = sut.FilterRows(rows);

        test.Count(x => x.ScoredResultRowId == testRow1.ScoredResultRowId).Should().Be(0);
        test.Count(x => x.ScoredResultRowId == testRow2.ScoredResultRowId).Should().Be(1);
        test.Count(x => x.ScoredResultRowId == testRow3.ScoredResultRowId).Should().Be(2);
    }

    private ColumnValueRowFilter CreateSut(string propertyName,
                                 IEnumerable<string> filterValues,
                                 ComparatorType comparator = ComparatorType.IsSmallerOrEqual,
                                 MatchedValueAction action = MatchedValueAction.Keep,
                                 bool allowForEach = false)
    {
        return new ColumnValueRowFilter(propertyName, comparator, filterValues, action, allowForEach);
    }
}
