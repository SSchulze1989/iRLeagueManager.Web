namespace iRLeagueApiCore.Services.ResultService.Models;

public sealed class ReviewPenaltyCalculationResult
{
    public long ReviewId { get; set; }
    public long? ReviewVoteId { get; set; }
    public int PenaltyPoints { get; set; }
}
