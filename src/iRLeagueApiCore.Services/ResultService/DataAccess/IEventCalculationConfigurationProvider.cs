using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal interface IEventCalculationConfigurationProvider
{
    public Task<IReadOnlyList<long>> GetResultConfigIds(long eventId, CancellationToken cancellationToken = default);
    public Task<EventCalculationConfiguration> GetConfiguration(long eventId, long? resultConfigId, CancellationToken cancellationToken = default);
}
