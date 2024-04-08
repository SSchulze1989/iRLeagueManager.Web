using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class PerPlacePointRule : CalculationPointRuleBase
{
    public IReadOnlyDictionary<int, double> PointsPerPlace { get; private set; }

    public PerPlacePointRule(IReadOnlyDictionary<int, double> pointPerPlace)
    {
        PointsPerPlace = pointPerPlace;
    }

    public override IReadOnlyList<T> ApplyPoints<T>(SessionCalculationData _, IReadOnlyList<T> rows)
    {
        foreach ((var row, var pos) in rows.Select((x, i) => (x, i + 1)))
        {
            row.RacePoints = PointsPerPlace.TryGetValue(pos, out double points) ? points : 0d;
        }
        return rows;
    }
}
