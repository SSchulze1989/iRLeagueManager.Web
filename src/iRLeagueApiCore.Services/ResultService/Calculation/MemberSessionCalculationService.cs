using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class MemberSessionCalculationService : CalculationServiceBase
{
    private readonly SessionCalculationConfiguration config;

    public MemberSessionCalculationService(SessionCalculationConfiguration config)
    {
        this.config = config;
    }

    public override Task<SessionCalculationResult> Calculate(SessionCalculationData data)
    {
        var rows = data.ResultRows
            .Select(row => new ResultRowCalculationResult(row))
            .Where(row => row.MemberId != null)
            .ToList();
        if (config.IsCombinedResult)
        {
            rows = CombineResults(rows, x => x.MemberId).ToList();
        }
        var pointRule = config.PointRule;
        var finalRows = ApplyPoints(rows, pointRule, data);

        var result = new SessionCalculationResult(data)
        {
            Name = config.Name,
            SessionResultId = config.SessionResultId,
            ResultRows = finalRows,
            SessionNr = data.SessionNr
        };
        (result.FastestAvgLapDriverMemberId, result.FastestAvgLap) = GetBestLapValue(finalRows, x => x.MemberId, x => x.AvgLapTime);
        (result.FastestLapDriverMemberId, result.FastestLap) = GetBestLapValue(finalRows, x => x.MemberId, x => x.FastestLapTime);
        (result.FastestQualyLapDriverMemberId, result.FastestQualyLap) = GetBestLapValue(finalRows, x => x.MemberId, x => x.QualifyingTime);
        result.CleanestDrivers = GetBestValues(rows, x => x.Incidents, x => x.MemberId, x => x.Min())
            .Select(x => x.id)
            .NotNull()
            .ToList();
        result.HardChargers = GetBestValues(rows.Where(HardChargerEligible), x => x.FinalPositionChange, x => x.MemberId, x => x.Max())
            .Select(x => x.id)
            .NotNull()
            .ToList();

        return Task.FromResult(result);
    }
}
