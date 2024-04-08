namespace iRLeagueApiCore.Services.ResultService.Models;

internal interface IPenaltyRow
{
    public IEnumerable<AddPenaltyCalculationData> AddPenalties { get; set; }
}
