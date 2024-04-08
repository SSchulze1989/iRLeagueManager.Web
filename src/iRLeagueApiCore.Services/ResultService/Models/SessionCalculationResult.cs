namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class SessionCalculationResult
{
    public SessionCalculationResult()
    {
    }

    public SessionCalculationResult(SessionCalculationData data)
    {
        LeagueId = data.LeagueId;
        SessionId = data.SessionId;
        Sof = data.Sof;
    }

    public long LeagueId { get; set; }
    public long? SessionId { get; set; }
    public int? SessionNr { get; set; }
    public long? SessionResultId { get; set; }
    public long? ScoringId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sof { get; set; }
    public TimeSpan FastestLap { get; set; }
    public TimeSpan FastestQualyLap { get; set; }
    public TimeSpan FastestAvgLap { get; set; }
    public long? FastestAvgLapDriverMemberId { get; set; }
    public long? FastestLapDriverMemberId { get; set; }
    public long? FastestQualyLapDriverMemberId { get; set; }
    public ICollection<long> CleanestDrivers { get; set; } = Array.Empty<long>();
    public ICollection<long> HardChargers { get; set; } = Array.Empty<long>();
    public IEnumerable<ResultRowCalculationResult> ResultRows { get; set; } = Array.Empty<ResultRowCalculationResult>();
}
