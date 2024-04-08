using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal interface IEventCalculationResultStore
{
    public Task StoreCalculationResult(EventCalculationResult result, CancellationToken cancellationToken = default);
}
