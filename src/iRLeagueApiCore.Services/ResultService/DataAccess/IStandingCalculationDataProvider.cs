using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal interface IStandingCalculationDataProvider
{
    public Task SetLeague(long eventId, CancellationToken cancellationToken);
    public Task<StandingCalculationData?> GetData(StandingCalculationConfiguration config, CancellationToken cancellationToken = default);
}
