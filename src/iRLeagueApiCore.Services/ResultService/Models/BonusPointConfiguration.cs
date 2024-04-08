using iRLeagueApiCore.Services.ResultService.Calculation;

namespace iRLeagueApiCore.Services.ResultService.Models;
internal sealed class BonusPointConfiguration
{
    public BonusPointType Type { get; set; }
    public int Value { get; set; }
    public int Points { get; set; }
    public FilterGroupRowFilter<ResultRowCalculationResult> Conditions { get; set; } = new();
}
