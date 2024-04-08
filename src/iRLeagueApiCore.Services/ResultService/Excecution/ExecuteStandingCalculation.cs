using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Excecution;

internal sealed class ExecuteStandingCalculation
{
    private readonly ILogger<ExecuteStandingCalculation> logger;
    private readonly IStandingCalculationDataProvider dataProvider;
    private readonly IStandingCalculationConfigurationProvider configProvider;
    private readonly IStandingCalculationResultStore dataStore;
    private readonly ICalculationServiceProvider<StandingCalculationConfiguration, StandingCalculationData, StandingCalculationResult> calculationServiceProvider;

    public ExecuteStandingCalculation(ILogger<ExecuteStandingCalculation> logger, IStandingCalculationDataProvider dataProvider,
        IStandingCalculationConfigurationProvider configProvider, IStandingCalculationResultStore dataStore,
        ICalculationServiceProvider<StandingCalculationConfiguration, StandingCalculationData, StandingCalculationResult> calculationServiceProvider)
    {
        this.logger = logger;
        this.dataProvider = dataProvider;
        this.configProvider = configProvider;
        this.dataStore = dataStore;
        this.calculationServiceProvider = calculationServiceProvider;
    }

    public async ValueTask Execute(long eventId, CancellationToken cancellationToken = default)
    {
        using var loggerScoppe = logger.BeginScope(new Dictionary<string, object> { ["ExecutionId"] = new Guid() });
        await dataProvider.SetLeague(eventId, cancellationToken);

        logger.LogInformation("--- Start standing calculation for event: {EventId} ---", eventId);
        var seasonId = await configProvider.GetSeasonId(eventId, cancellationToken) ?? -1;
        if (seasonId == -1)
        {
            logger.LogInformation("No season found for event {EvenId} - cancelling standing calculation", eventId);
            return;
        }
        IEnumerable<long?> standingConfigIds = (await configProvider.GetStandingConfigIds(seasonId, cancellationToken)).Cast<long?>();
        if (standingConfigIds.Any() == false)
        {
            standingConfigIds = new[] { default(long?) };
            logger.LogInformation("No standing config found -> Using default.");
        }

        var standingCount = 0;
        logger.LogInformation("Calculating standings for config ids: [{StandingConfigIds}]", standingConfigIds);
        try
        {
            foreach (var configId in standingConfigIds)
            {
                try
                {
                    var config = await configProvider.GetConfiguration(seasonId, eventId, configId, cancellationToken);
                    var data = await dataProvider.GetData(config, cancellationToken);
                    if (data == null)
                    {
                        logger.LogInformation("No standing data available for event: {EventId} | config: {ConfigId}", eventId, configId);
                        continue;
                    }
                    var calculationService = calculationServiceProvider.GetCalculationService(config);
                    var result = await calculationService.Calculate(data);
                    logger.LogInformation("Standing calculated for event: {EventId} | config: {ConfigId}\n" +
                        " - StandingRows: {StandingRowCount}", eventId, configId, result.StandingRows.Count);
                    await dataStore.StoreCalculationResult(result, cancellationToken);
                    standingCount++;
                    logger.LogInformation("Standing stored");
                }
                catch (Exception ex) when (ex is AggregateException || ex is InvalidOperationException || ex is NotImplementedException)
                {
                    logger.LogError("Error thrown while calculating standing for configId ({ConfigId}): {Exception}", configId, ex);
                }
            }
            logger.LogInformation("Standings calculated for season: {SeasonId}, event: {EventId}\n" +
                " - Standings: {StandingCount}", seasonId, eventId, standingCount);
            await dataStore.ClearStaleStandings(standingConfigIds, eventId);
            logger.LogInformation("Cleared stale Standings");
            logger.LogInformation("--- Standing calculation finished successfully ---");
        }
        catch (Exception ex) when (ex is AggregateException || ex is InvalidOperationException || ex is NotImplementedException)
        {
            logger.LogError("Error thrown while calculating standings: {Exception}", ex);
        }
    }
}
