using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Mocking.Extensions;
using iRLeagueApiCore.Services.ResultService.Calculation;
using System.Diagnostics;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;
public sealed class FilterGroupRowFilterTests
{
    private readonly Fixture fixture = new();

    [Fact]
    public void FilterRows_ShouldFilter_WithSingleFilter()
    {
        var pos = Enumerable.Range(1, 10);
        var rows = fixture.Build<TestRow>()
            .With(x => x.Value1, pos.CreateSequence())
            .CreateMany(10)
            .ToList();
        Debug.Assert(rows[1].Value1 == 2);
        var filter = TestFilter<TestRow>(x => x.Value1 <= 5);
        Debug.Assert(filter.FilterRows(rows).Count() == 5);
        var filterGroup = new FilterGroupRowFilter<TestRow>(new[] { (FilterCombination.And, filter) });

        var testRows = filterGroup.FilterRows(rows);

        testRows.Should().HaveCount(5);
        testRows.Select(x => x.Value1).Should().BeEquivalentTo(pos.Take(5));
    }

    [Fact] 
    public void FilterRows_ShouldFilter_MultipleAndFilters()
    {
        var pos = Enumerable.Range(1, 10);
        var rows = fixture.Build<TestRow>()
            .With(x => x.Value1, pos.CreateSequence())
            .CreateMany(10)
            .ToList();
        Debug.Assert(rows[1].Value1 == 2);
        var filter1 = TestFilter<TestRow>(x => x.Value1 <= 5);
        var filter2 = TestFilter<TestRow>(x => x.Value1 > 2);
        Debug.Assert(filter1.FilterRows(rows).Count() == 5);
        Debug.Assert(filter2.FilterRows(rows).Count() == 8);
        var filterGroup = new FilterGroupRowFilter<TestRow>(new[] { (FilterCombination.And, filter1), (FilterCombination.And, filter2) });

        var testRows = filterGroup.FilterRows(rows);

        testRows.Should().HaveCount(3);
        testRows.Select(x => x.Value1).Should().BeEquivalentTo(pos.Skip(2).Take(3));
    }

    [Fact]
    public void FilterRows_ShouldFilter_MultipleAndAndOrFilters()
    {
        var pos = Enumerable.Range(1, 10);
        var rating = new[] { 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 1.0, 1.0 };
        var rows = fixture.Build<TestRow>()
            .With(x => x.Value1, pos.CreateSequence())
            .With(x => x.Value2, rating.CreateSequence())
            .CreateMany(10)
            .ToList();
        Debug.Assert(rows[1].Value1 == 2);
        var filter1 = TestFilter<TestRow>(x => x.Value1 <= 3);
        var filter2 = TestFilter<TestRow>(x => x.Value2 == 1.0);
        Debug.Assert(filter1.FilterRows(rows).Count() == 3);
        Debug.Assert(filter2.FilterRows(rows).Count() == 2);
        var filterGroup = new FilterGroupRowFilter<TestRow>(new[] { (FilterCombination.And, filter1), (FilterCombination.Or, filter2) });

        var testRows = filterGroup.FilterRows(rows);

        testRows.Should().HaveCount(5);
        testRows.Take(3).Select(x => x.Value1).Should().BeEquivalentTo(pos.Take(3));
        testRows.Skip(3).Select(x => x.Value1).Should().BeEquivalentTo(pos.Skip(8));
    }

    [Fact]
    public void FilterRows_ShouldNotChangeOrder()
    {
        var pos = Enumerable.Range(1, 10);
        var rating = new[] { 1.0, 1.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0 };
        var rows = fixture.Build<TestRow>()
            .With(x => x.Value1, pos.CreateSequence())
            .With(x => x.Value2, rating.CreateSequence())
            .CreateMany(10)
            .ToList();
        Debug.Assert(rows[1].Value1 == 2);
        var filter1 = TestFilter<TestRow>(x => x.Value1 > 7);
        var filter2 = TestFilter<TestRow>(x => x.Value2 == 1.0);
        Debug.Assert(filter1.FilterRows(rows).Count() == 3);
        Debug.Assert(filter2.FilterRows(rows).Count() == 2);
        var filterGroup = new FilterGroupRowFilter<TestRow>(new[] { (FilterCombination.And, filter1), (FilterCombination.Or, filter2) });

        var testRows = filterGroup.FilterRows(rows);

        testRows.Should().HaveCount(5);
        testRows.Take(2).Select(x => x.Value1).Should().BeEquivalentTo(pos.Take(2));
        testRows.Skip(2).Select(x => x.Value1).Should().BeEquivalentTo(pos.Skip(7));
    }

    private static RowFilter<T> TestFilter<T>(Func<T, bool> filterFunc)
    {
        var filter = new Mock<RowFilter<T>>();
        filter.Setup(x => x.FilterRows(It.IsAny<IEnumerable<T>>()))
            .Returns((IEnumerable<T> rows) => rows.Where(filterFunc));
        return filter.Object;
    }

    public record TestRow(int Value1, double Value2);
}


