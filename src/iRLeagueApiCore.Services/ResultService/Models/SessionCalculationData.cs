namespace iRLeagueApiCore.Services.ResultService.Models;

internal sealed class SessionCalculationData
{
    public long LeagueId { get; set; }
    public long? SessionId { get; set; }
    public int? SessionNr { get; set; }
    public int Sof { get; set; }
    public TimeSpan FastestLap { get; set; }
    public TimeSpan FastestQualyLap { get; set; }
    public TimeSpan FastestAvgLap { get; set; }
    public IEnumerable<AcceptedReviewVoteCalculationData> AcceptedReviewVotes { get; set; } = Array.Empty<AcceptedReviewVoteCalculationData>();
    public IEnumerable<ResultRowCalculationData> ResultRows { get; set; } = Array.Empty<ResultRowCalculationData>();
}
