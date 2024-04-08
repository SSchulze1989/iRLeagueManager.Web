using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class EventCalculationConfigurationProvider : DatabaseAccessBase, IEventCalculationConfigurationProvider
{
    private readonly ISessionCalculationConfigurationProvider sessionConfigurationProvider;
    public EventCalculationConfigurationProvider(LeagueDbContext dbContext,
        ISessionCalculationConfigurationProvider sessionConfigurationProvider) :
        base(dbContext)
    {
        this.sessionConfigurationProvider = sessionConfigurationProvider;
    }

    public async Task<IReadOnlyList<long>> GetResultConfigIds(long eventId, CancellationToken cancellationToken = default)
    {
        var configs = await dbContext.Events
            .Where(x => x.EventId == eventId)
            .SelectMany(x => x.ResultConfigs.Select(y => new { y.ResultConfigId, y.SourceResultConfigId }))
            .ToListAsync(cancellationToken);
        return SortInOrderOfDependency(configs.Select(x => (x.ResultConfigId, x.SourceResultConfigId)));
    }

    public async Task<EventCalculationConfiguration> GetConfiguration(long eventId, long? resultConfigId, CancellationToken cancellationToken = default)
    {
        var configEntity = await GetResultConfigurationEntity(resultConfigId, cancellationToken);
        var eventEntity = await GetEventEntity(eventId, resultConfigId, cancellationToken);
        var champSeasonEntity = await GetChampSeasonEntity(eventEntity, configEntity, cancellationToken);
        if (configEntity is not null)
        {
            configEntity.ChampSeason = champSeasonEntity;
        }
        var resultConfiguration = await GetEventResultCalculationConfiguration(eventEntity, champSeasonEntity, configEntity, cancellationToken);
        return resultConfiguration;
    }

    private async Task<ChampSeasonEntity?> GetChampSeasonEntity(EventEntity eventEntity, ResultConfigurationEntity? configEntity, CancellationToken cancellationToken)
    {
        if (configEntity is null)
        {
            return null;
        }

        return await dbContext.ChampSeasons
            .Include(x => x.Championship)
            .Include(x => x.Filters)
            .Where(x => x.SeasonId == eventEntity.Schedule.SeasonId)
            .Where(x => x.ResultConfigurations.Contains(configEntity))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<ResultConfigurationEntity?> GetResultConfigurationEntity(long? resultConfigId, CancellationToken cancellationToken)
    {
        if (resultConfigId == null)
        {
            return null;
        }

        return await dbContext.ResultConfigurations
            .Include(x => x.Scorings)
                .ThenInclude(x => x.PointsRule)
                    .ThenInclude(x => x.AutoPenalties)
            .Include(x => x.Scorings)
                .ThenInclude(x => x.ExtScoringSource)
            .Include(x => x.PointFilters)
            .Include(x => x.ResultFilters)
            .FirstOrDefaultAsync(x => x.ResultConfigId == resultConfigId, cancellationToken)
            ?? throw new InvalidOperationException($"No result configuration with id:{resultConfigId} found");
    }

    private async Task<EventEntity> GetEventEntity(long eventId, long? resultConfigId, CancellationToken cancellationToken)
    {
        var events = dbContext.Events
                .Include(x => x.Schedule)
                .Include(x => x.Sessions);

        if (resultConfigId == null)
        {
            return await events
                .Where(x => x.EventId == eventId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException($"No event id:{eventId} found in database");
        }

        return await events
            .Where(x => x.EventId == eventId)
            .Where(x => x.ResultConfigs.Any(y => y.ResultConfigId == resultConfigId))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"No event id:{eventId} registered with result configuration id:{resultConfigId}");
    }

    private async Task<EventCalculationConfiguration> GetEventResultCalculationConfiguration(EventEntity eventEntity, ChampSeasonEntity? champSeason, ResultConfigurationEntity? configEntity,
        CancellationToken cancellationToken)
    {
        EventCalculationConfiguration configuration = new();
        var configId = configEntity?.ResultConfigId;
        var championship = champSeason?.Championship;
        configuration.EventId = eventEntity.EventId;
        configuration.LeagueId = eventEntity.LeagueId;
        configuration.ResultId = await dbContext.ScoredEventResults
            .Where(x => x.ResultConfigId == configId)
            .Where(x => x.EventId == eventEntity.EventId)
            .Select(x => x.ResultId)
            .FirstOrDefaultAsync(cancellationToken);
        configuration.ChampSeasonId = champSeason?.ChampSeasonId;
        configuration.ResultConfigId = configId;
        configuration.SourceResultConfigId = configEntity?.SourceResultConfigId;
        configuration.DisplayName = (string.IsNullOrWhiteSpace(championship?.DisplayName) ? championship?.Name : championship.DisplayName) ?? "Default";
        configuration.SessionResultConfigurations = await sessionConfigurationProvider.GetConfigurations(eventEntity, configEntity, cancellationToken);
        return configuration;
    }

    private static IReadOnlyList<long> SortInOrderOfDependency(IEnumerable<(long id, long? sourceId)> configs)
    {
        // Implementation of Kahn's algorithm --> see https://en.wikipedia.org/wiki/Topological_sorting
        var sortList = new List<long>();
        if (configs.Any() == false)
        {
            return sortList;
        }

        var source = configs.ToDictionary(k => k.id, v => v.sourceId);
        var startNodes = configs.Where(x => x.sourceId is null).ToList();
        if (startNodes.Any() == false || startNodes.Any(x => x.id == x.sourceId))
        {
            throw new InvalidOperationException("ResultConfiguration list contains cyclic dependencies");
        }

        while (startNodes.Any())
        {
            var node = startNodes.First();
            startNodes.Remove(node);

            sortList.Add(node.id);
            foreach (var (id, sourceId) in source.Where(x => x.Value == node.id))
            {
                source[id] = null;
                startNodes.Add((id, null));
            }
        }

        if (source.Values.Any(x => x is not null))
        {
            throw new InvalidOperationException("ResultConfiguration list contains cyclic dependencies, or dependencies outside of this event scope");
        }

        return sortList;
    }
}
