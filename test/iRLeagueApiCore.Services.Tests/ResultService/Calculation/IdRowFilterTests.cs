using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;
public sealed class IdRowFilterTests
{
    private static readonly Fixture fixture = new();

    private static IEnumerable<object[]> TestValidConstructorData() => new[]
        {
            new object[] { fixture.CreateMany<long>().Select(x => x.ToString()) },
        };

    private static IEnumerable<object[]> TestInvalidConstructorData() => new[]
        {
            new object[] { new[] { "123a4" } },
            new object[] { new[] { fixture.Create<Guid>().ToString() } },
            new object[] { new[] { "1.23" } },
        };

    [Theory]
    [MemberData(nameof(TestValidConstructorData))]
    public void Constructor_ShouldNotThrow_WithValidValues(IEnumerable<string> values)
    {
        _ = new IdRowFilter<long>(values, x => x.MemberId.GetValueOrDefault(), MatchedValueAction.Keep);
    }

    [Theory]
    [MemberData(nameof(TestInvalidConstructorData))]
    public void Constructor_ShouldThrow_WithInvalidValues(IEnumerable<string> values)
    {
        var test = () => new IdRowFilter<long>(values, x => x.MemberId.GetValueOrDefault(), MatchedValueAction.Keep);
        test.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FilterValues_ShouldFilterValues_WithKeepAction()
    {
        int valueCount = 3;
        var rows = fixture.CreateMany<ResultRowCalculationResult>(10).ToList();
        var memberIds = rows.Select(x => x.MemberId).Take(valueCount).ToList();
        rows = rows.Shuffle().ToList();
        var sut = new IdRowFilter<long>(memberIds.Select(x => x.ToString()).NotNull(), x => x.MemberId.GetValueOrDefault(), MatchedValueAction.Keep);

        var test = sut.FilterRows(rows);

        test.Should().HaveCount(valueCount);
        foreach(var row in test)
        {
            memberIds.Should().Contain(row.MemberId);
        }
    }

    [Fact]
    public void FilterValues_ShouldFilterValues_WithRemoveAction()
    {
        int valueCount = 3;
        var rows = fixture.CreateMany<ResultRowCalculationResult>(10).ToList();
        var memberIds = rows.Select(x => x.MemberId).Take(valueCount).ToList();
        rows = rows.Shuffle().ToList();
        var sut = new IdRowFilter<long>(memberIds.Select(x => x.ToString()).NotNull(), x => x.MemberId.GetValueOrDefault(), MatchedValueAction.Remove);

        var test = sut.FilterRows(rows);

        test.Should().HaveCount(10 - valueCount);
        foreach (var row in test)
        {
            memberIds.Should().NotContain(row.MemberId);
        }
    }
}
