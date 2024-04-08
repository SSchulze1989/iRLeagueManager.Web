using iRLeagueApiCore.Common.Enums;

namespace iRLeagueApiCore.Services.ResultService.Models;

public sealed class AddPenaltyCalculationData
{ 
    public int SessionNr { get; set; }
    public long? MemberId { get; set; }
    public long? TeamId { get; set; }
    public PenaltyType Type { get; set; }
    public double Points { get; set; }
    public int Positions { get; set; }
    public TimeSpan Time { get; set; }
}
