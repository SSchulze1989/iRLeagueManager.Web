using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class MaxPointRule : CalculationPointRuleBase
{
    public int MaxPoints { get; private set; }
    public int DropOff { get; private set; }

    public MaxPointRule(int maxPoints, int dropOff)
    {
        MaxPoints = maxPoints;
        DropOff = dropOff;
    }

    public override IReadOnlyList<T> ApplyPoints<T>(SessionCalculationData _, IReadOnlyList<T> rows)
    {
        foreach ((var row, var pos) in rows.Select((x, i) => (x, i + 1)))
        {
            row.RacePoints = GetPointsForPosition(pos);
        }
        return rows;
    }

    private int GetPointsForPosition(int pos)
    {
        return Math.Max(MaxPoints - (pos - 1) * DropOff, 0);
    }
}
