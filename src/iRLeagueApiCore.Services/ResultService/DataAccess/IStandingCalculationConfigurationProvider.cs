using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal interface IStandingCalculationConfigurationProvider
{
    public Task<long?> GetSeasonId(long eventId, CancellationToken cancellationToken);
    public Task<IReadOnlyList<long>> GetStandingConfigIds(long seasonId, CancellationToken cancellationToken = default);
    public Task<StandingCalculationConfiguration> GetConfiguration(long seasonId, long? eventId, long? resultConfigId, CancellationToken cancellationToken = default);
}
