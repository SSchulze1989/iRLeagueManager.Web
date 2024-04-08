using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class StandingCalculationConfigurationProvider : DatabaseAccessBase, IStandingCalculationConfigurationProvider
{
    public StandingCalculationConfigurationProvider(LeagueDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<long?> GetSeasonId(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Where(x => x.EventId == eventId)
            .Select(x => x.Schedule.SeasonId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<long>> GetStandingConfigIds(long seasonId, CancellationToken cancellationToken = default)
    {
        var configIds = await dbContext.ChampSeasons
            .Where(x => x.SeasonId == seasonId)
            .Where(x => x.StandingConfigId != null)
            .Where(x => x.IsActive)
            .Select(x => x.StandingConfigId.GetValueOrDefault())
            .Distinct()
            .ToListAsync(cancellationToken);
        return configIds;
    }

    public async Task<StandingCalculationConfiguration> GetConfiguration(long seasonId, long? eventId, long? standingConfigId, CancellationToken cancellationToken = default)
    {
        var season = await GetSeasonEntityAsync(seasonId, cancellationToken)
            ?? throw new ArgumentException($"No season with id {seasonId} found", nameof(seasonId));
        var @event = await GetEventEntityAsync(seasonId, eventId, cancellationToken)
            ?? await GetLatestEventEntityAsync(seasonId, cancellationToken);
        if (@event is null)
        {
            return EmptyStandingConfiguration();
        }
        var config = DefaultStandingConfiguration(season, @event.EventId);
        var standingConfig = await GetConfigurationEntityAsync(standingConfigId, cancellationToken);
        var champSeason = standingConfig?.ChampSeasons.FirstOrDefault(x => x.SeasonId == seasonId);
        if (standingConfig is not null && champSeason is not null)
        {
            var championship = champSeason.Championship;
            config.ChampSeasonId = standingConfig.ChampSeasons.FirstOrDefault(x => x.SeasonId == seasonId)?.ChampSeasonId;
            config.StandingConfigId = standingConfig.StandingConfigId;
            config.ResultConfigs = champSeason.ResultConfigurations.Select(x => x.ResultConfigId);
            config.Name = champSeason.Championship.Name;
            config.DisplayName = string.IsNullOrWhiteSpace(championship.DisplayName) ? championship.Name : championship.DisplayName;
            config.UseCombinedResult = standingConfig.UseCombinedResult;
            config.ResultKind = champSeason.ResultKind;
            config.WeeksCounted = standingConfig.WeeksCounted > 0 ? standingConfig.WeeksCounted : 999;
            config.SortOptions = standingConfig.SortOptions;
        }
        // Add default sorting options if none are configured
        if (config.SortOptions.None())
        {
            config.SortOptions.Add(SortOptions.TotalPtsDesc);
            config.SortOptions.Add(SortOptions.PenPtsAsc);
            config.SortOptions.Add(SortOptions.WinsDesc);
            config.SortOptions.Add(SortOptions.IncsAsc);
        }
        return config;
    }

    private async Task<SeasonEntity?> GetSeasonEntityAsync(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.Seasons
            .FirstOrDefaultAsync(x => x.SeasonId == seasonId, cancellationToken);
    }

    private async Task<StandingConfigurationEntity?> GetConfigurationEntityAsync(long? standingConfigId, CancellationToken cancellationToken)
    {
        return await dbContext.StandingConfigurations
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.ResultConfigurations)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.Championship)
            .FirstOrDefaultAsync(x => x.StandingConfigId == standingConfigId, cancellationToken: cancellationToken);
    }

    private async Task<EventEntity?> GetEventEntityAsync(long seasonId, long? eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Where(x => x.Schedule.SeasonId == seasonId)
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
    }

    private async Task<EventEntity?> GetLatestEventEntityAsync(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Where(x => x.Schedule.SeasonId == seasonId)
            .Where(x => x.ScoredEventResults.Any())
            .OrderBy(x => x.Date)
            .LastOrDefaultAsync(cancellationToken);
    }

    private static StandingCalculationConfiguration DefaultStandingConfiguration(SeasonEntity season, long eventId)
    {
        return new StandingCalculationConfiguration()
        {
            Name = "Default",
            DisplayName = "Default",
            LeagueId = season.LeagueId,
            SeasonId = season.SeasonId,
            EventId = eventId,
        };
    }

    private static StandingCalculationConfiguration EmptyStandingConfiguration()
    {
        return new StandingCalculationConfiguration();
    }
}
