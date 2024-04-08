using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class UseResultPointsPointRule : CalculationPointRuleBase
{
    public override IReadOnlyList<T> ApplyPoints<T>(SessionCalculationData _, IReadOnlyList<T> rows)
    {
        return rows;
    }
}
