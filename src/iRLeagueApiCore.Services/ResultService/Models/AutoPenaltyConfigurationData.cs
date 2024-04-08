using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Calculation;

namespace iRLeagueApiCore.Services.ResultService.Models;
internal sealed class AutoPenaltyConfigurationData
{
    public string Description { get; set; } = string.Empty;
    public PenaltyType Type { get; set; }
    public int Points { get; set; }
    public TimeSpan Time { get; set; }
    public int Positions { get; set; }
    public FilterGroupRowFilter<ResultRowCalculationResult> Conditions { get; set; } = new();
}
