using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace iRLeagueApiCore.Services.ResultService.Excecution;

internal sealed class ExecuteEventResultCalculation
{
    private readonly ILogger<ExecuteEventResultCalculation> logger;
    private readonly IEventCalculationDataProvider dataProvider;
    private readonly IEventCalculationConfigurationProvider configProvider;
    private readonly IEventCalculationResultStore dataStore;
    private readonly ICalculationServiceProvider<EventCalculationConfiguration, EventCalculationData, EventCalculationResult> calculationServiceProvider;
    private readonly IStandingCalculationQueue standingCalculationQueue;

    public ExecuteEventResultCalculation(ILogger<ExecuteEventResultCalculation> logger,
        IEventCalculationDataProvider dataProvider,
        IEventCalculationConfigurationProvider configProvider,
        IEventCalculationResultStore dataStore,
        ICalculationServiceProvider<EventCalculationConfiguration, EventCalculationData, EventCalculationResult> calculationServiceProvider,
        IStandingCalculationQueue standingCalculationQueue)
    {
        this.logger = logger;
        this.dataProvider = dataProvider;
        this.configProvider = configProvider;
        this.dataStore = dataStore;
        this.calculationServiceProvider = calculationServiceProvider;
        this.standingCalculationQueue = standingCalculationQueue;
    }

    public async ValueTask Execute(long eventId, CancellationToken cancellationToken = default)
    {
        using var loggerScoppe = logger.BeginScope(new Dictionary<string, object> { ["ExecutionId"] = new Guid() });

        logger.LogInformation("--- Start result calculation for event: {EventId} ---", eventId);
        await dataProvider.SetLeague(eventId, cancellationToken);

        IEnumerable<long?> resultConfigIds = (await configProvider.GetResultConfigIds(eventId, cancellationToken)).Cast<long?>();
        if (resultConfigIds.Any() == false)
        {
            resultConfigIds = new[] { default(long?) };
            logger.LogInformation("No result config found -> Using default.");
        }

        var eventResultCount = 0;
        logger.LogInformation("Calculating results for config ids: [{ResultConfigIds}]", resultConfigIds);
        try
        {
            foreach (var configId in resultConfigIds)
            {
                try
                {
                    var config = await configProvider.GetConfiguration(eventId, configId, cancellationToken);
                    var data = await dataProvider.GetData(config, cancellationToken);
                    if (data == null)
                    {
                        logger.LogInformation("No result data available for event: {EventId} | config: {ConfigId}", eventId, configId);
                        continue;
                    }
                    var calculationService = calculationServiceProvider.GetCalculationService(config);
                    var result = await calculationService.Calculate(data);
                    logger.LogInformation("Result calculated for event: {EventId} | config: {ConfigId}\n" +
                        " - SessionResults: {SessionResultCount}\n" +
                        " - ResultRows: {ResultRowCount}", eventId, configId, result.SessionResults.Count(), result.SessionResults.SelectMany(x => x.ResultRows).Count());
                    await dataStore.StoreCalculationResult(result, cancellationToken);
                    eventResultCount++;
                    logger.LogInformation("Results stored");
                }
                catch (Exception ex) when (ex is AggregateException || ex is InvalidOperationException || ex is NotImplementedException)
                {
                    logger.LogError("Error thrown while calculating results for configId ({ConfigId}): {Exception}", configId, ex);
                }
            }
            logger.LogInformation("Results calculated for event: {EventId}\n" +
                " - EventResults: {EventResultCount}", eventId, eventResultCount);
            logger.LogInformation("--- Result calculation finished successfully ---");
        }
        catch (Exception ex) when (ex is AggregateException || ex is InvalidOperationException || ex is NotImplementedException)
        {
            logger.LogError("Error thrown while calculating results: {Exception}", ex);
        }


        logger.LogInformation("Queueing standing calculation for event {EventId}", eventId);
        await standingCalculationQueue.QueueStandingCalculationAsync(eventId);
    }
}
