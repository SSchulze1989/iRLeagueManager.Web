namespace iRLeagueApiCore.Services.ResultService.Models;

internal interface IPointRow
{
    public double RacePoints { get; set; }
    public double BonusPoints { get; set; }
    public double PenaltyPoints { get; set; }
    public double TotalPoints { get; set; }
}
