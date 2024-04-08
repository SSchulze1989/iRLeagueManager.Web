using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Common.Tests;

public sealed class IntervalTests
{
    public static IEnumerable<object[]> ConstructorTestData => new[] {
        new object[] { new TimeSpan(0, 0, 2, 3), new Interval() { Time = new TimeSpan(0, 2, 3), Laps = 0 } },
        new object[] { new TimeSpan(1, 0, 0, 0), new Interval() { Time = new TimeSpan(0, 0, 0), Laps = 1 } },
        new object[] { new TimeSpan(2, 0, 2, 3), new Interval() { Time = new TimeSpan(0, 2, 3), Laps = 2 } },
    };

    public static IEnumerable<object[]> CompareTestData => new[]
    {
        new object[] { new Interval(new TimeSpan(1, 2, 3)), new Interval(new TimeSpan(3, 2, 1)), -1 },
        new object[] { new Interval(new TimeSpan(3, 2, 1)), new Interval(new TimeSpan(3, 2, 1)), 0 },
        new object[] { new Interval(new TimeSpan(3, 2, 1)), new Interval(new TimeSpan(1, 2, 3)), 1 },
        new object[] { new Interval(new TimeSpan(1, 0, 0, 0)), new Interval(new TimeSpan(2, 0, 0, 0)), -1 },
        new object[] { new Interval(new TimeSpan(2, 0, 0, 0)), new Interval(new TimeSpan(2, 0, 0, 0)), 0 },
        new object[] { new Interval(new TimeSpan(2, 0, 0, 0)), new Interval(new TimeSpan(1, 0, 0, 0)), 1 },
        new object[] { new Interval(new TimeSpan(1, 0, 2, 3)), new Interval(new TimeSpan(2, 0, 2, 3)), -1 },
        new object[] { new Interval(new TimeSpan(2, 0, 2, 3)), new Interval(new TimeSpan(2, 0, 2, 3)), 0 },
        new object[] { new Interval(new TimeSpan(2, 0, 2, 3)), new Interval(new TimeSpan(1, 0, 2, 3)), 1 },
        new object[] { new Interval(new TimeSpan(1, 0, 2, 3)), new Interval(new TimeSpan(1, 0, 3, 2)), -1 },
        new object[] { new Interval(new TimeSpan(1, 0, 3, 2)), new Interval(new TimeSpan(1, 0, 3, 2)), 0 },
        new object[] { new Interval(new TimeSpan(1, 0, 3, 2)), new Interval(new TimeSpan(1, 0, 2, 3)), 1 },
    };

    [Theory]
    [MemberData(nameof(ConstructorTestData))]
    public void Interval_ShouldConstructFromTimeSpan(TimeSpan time, Interval expected)
    {
        var test = new Interval(time);

        test.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(CompareTestData))]
    public void Interval_ShouldCompareToOtherInterval(Interval left, Interval right, int expected)
    {
        switch (expected)
        {
            case -1:
                left.Should().BeLessThan(right);
                break;
            case 0:
                left.Should().BeEquivalentTo(right);
                break;
            case 1:
                left.Should().BeGreaterThan(right);
                break;
        }
    }
}
