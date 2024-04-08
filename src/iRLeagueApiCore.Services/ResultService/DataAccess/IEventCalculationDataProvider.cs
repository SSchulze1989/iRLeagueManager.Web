using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal interface IEventCalculationDataProvider
{
    public Task SetLeague(long eventId, CancellationToken cancellationToken);
    public Task<EventCalculationData?> GetData(EventCalculationConfiguration config, CancellationToken cancellationToken);
}
