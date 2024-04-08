using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal class StandingCalculationResultStore : DatabaseAccessBase, IStandingCalculationResultStore
{
    public StandingCalculationResultStore(LeagueDbContext dbContext) : base(dbContext)
    {
    }

    public async Task StoreCalculationResult(StandingCalculationResult result, CancellationToken cancellationToken = default)
    {
        var standing = await GetOrCreateStandingEntity(result, cancellationToken);
        var requiredEntities = await GetRequiredEntities(result, cancellationToken);
        await MapToStandingEntity(result, standing, requiredEntities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<StandingEntity> GetOrCreateStandingEntity(StandingCalculationResult result, CancellationToken cancellationToken)
    {
        var standing = await dbContext.Standings
            .Include(x => x.StandingRows)
                .ThenInclude(x => x.ResultRows)
                    .ThenInclude(x => x.ScoredResultRow)
            .Where(x => x.EventId == result.EventId)
            .Where(x => x.StandingConfigId == result.StandingConfigId)
            .FirstOrDefaultAsync(cancellationToken);
        if (standing is null)
        {
            var @event = await dbContext.Events
                .Where(x => x.LeagueId == result.LeagueId)
                .FirstOrDefaultAsync(x => x.EventId == result.EventId, cancellationToken)
                ?? throw new InvalidOperationException($"No event with id {result.EventId} found");
            var season = await dbContext.Seasons
                .Where(x => x.LeagueId == result.LeagueId)
                .FirstOrDefaultAsync(x => x.SeasonId == result.SeasonId, cancellationToken)
                ?? throw new InvalidOperationException($"No season with id {result.SeasonId} found");
            var standingConfig = await dbContext.StandingConfigurations
                .Where(x => x.LeagueId == result.LeagueId)
                .Where(x => x.StandingConfigId == result.StandingConfigId)
                .FirstOrDefaultAsync(cancellationToken);
            standing = new StandingEntity()
            {
                Season = season,
                Event = @event,
                StandingConfig = standingConfig,
            };
            dbContext.Standings.Add(standing);
        }
        return standing;
    }

    private async Task<StandingEntity> MapToStandingEntity(StandingCalculationResult result, StandingEntity entity, RequiredEntities requiredEntities,
        CancellationToken cancellationToken)
    {
        entity.ChampSeason = await dbContext.ChampSeasons
            .FirstOrDefaultAsync(x => x.ChampSeasonId == result.ChampSeasonId, cancellationToken);
        entity.Name = result.Name;
        entity.IsTeamStanding = result.IsTeamStanding;
        foreach (var row in result.StandingRows)
        {
            StandingRowEntity? rowEntity = default;
            if (row.MemberId is not null)
            {
                rowEntity = entity.StandingRows
                    .FirstOrDefault(x => x.MemberId == row.MemberId);
            }
            if (rowEntity is null && row.MemberId is null && row.TeamId is not null)
            {
                rowEntity = entity.StandingRows
                    .FirstOrDefault(x => x.TeamId == row.TeamId);
            }
            if (rowEntity is null)
            {
                rowEntity = new()
                {
                    Member = requiredEntities.Members.FirstOrDefault(x => x.Id == row.MemberId),
                    MemberId = row.MemberId,
                    Team = requiredEntities.Teams.FirstOrDefault(x => x.TeamId == row.TeamId),
                    TeamId = row.TeamId,
                };
                entity.StandingRows.Add(rowEntity);
            }
            rowEntity.CarClass = row.CarClass;
            rowEntity.ClassId = row.ClassId;
            rowEntity.CompletedLaps = row.CompletedLaps;
            rowEntity.CompletedLapsChange = row.CompletedLapsChange;
            rowEntity.DroppedResultCount = row.DroppedResultCount;
            rowEntity.FastestLaps = row.FastestLaps;
            rowEntity.FastestLapsChange = row.FastestLapsChange;
            rowEntity.Incidents = row.Incidents;
            rowEntity.IncidentsChange = row.IncidentsChange;
            rowEntity.LastPosition = row.LastPosition;
            rowEntity.LeadLaps = row.LeadLaps;
            rowEntity.LeadLapsChange = row.LeadLapsChange;
            rowEntity.PenaltyPoints = row.PenaltyPoints;
            rowEntity.PenaltyPointsChange = row.PenaltyPointsChange;
            rowEntity.PolePositions = row.PolePositions;
            rowEntity.PolePositionsChange = row.PolePositionsChange;
            rowEntity.Position = row.Position;
            rowEntity.PositionChange = row.PositionChange;
            rowEntity.RacePoints = row.RacePoints;
            rowEntity.RacePointsChange = row.RacePointsChange;
            rowEntity.Races = row.Races;
            rowEntity.RacesCounted = row.RacesCounted;
            rowEntity.RacesScored = row.RacesScored;
            rowEntity.RacesInPoints = row.RacesInPoints;
            rowEntity.Team = requiredEntities.Teams.FirstOrDefault(x => x.TeamId == row.TeamId);
            rowEntity.Top10 = row.Top10;
            rowEntity.Top3 = row.Top3;
            rowEntity.Top5 = row.Top5;
            rowEntity.TotalPoints = row.TotalPoints;
            rowEntity.TotalPointsChange = row.TotalPointsChange;
            rowEntity.Wins = row.Wins;
            rowEntity.WinsChange = row.WinsChange;
            rowEntity.ResultRows = MapToStandingResultRowsList(result.LeagueId, row.ResultRows, rowEntity.ResultRows, requiredEntities);
            rowEntity.StartIrating = row.StartIrating;
            rowEntity.LastIrating = row.LastIrating;
        }

        var memberIds = result.StandingRows.Select(x => x.MemberId);
        foreach (var row in entity.StandingRows.Where(x => memberIds.Contains(x.MemberId) == false))
        {
            entity.StandingRows.Remove(row);
        }

        return entity;
    }

    private ICollection<StandingRows_ScoredResultRows> MapToStandingResultRowsList(long leagueId, IEnumerable<ResultRowCalculationResult> rowsData,
        ICollection<StandingRows_ScoredResultRows> rowsEntities, RequiredEntities requiredEntities)
    {
        foreach (var row in rowsData.Where(x => x.ScoredResultRowId != null))
        {
            var rowEntity = rowsEntities
                .Where(x => x.LeagueId == leagueId)
                .Where(x => x.ScoredResultRowRefId == row.ScoredResultRowId)
                .FirstOrDefault();
            if (rowEntity is null)
            {
                rowEntity = new()
                {
                    LeagueId = leagueId,
                    ScoredResultRow = requiredEntities.ScoredResultRows.First(x => x.ScoredResultRowId == row.ScoredResultRowId)
                };
                rowsEntities.Add(rowEntity);
            }
            rowEntity.IsScored = row.IsScored;
        }
        return rowsEntities;
    }

    private async Task<RequiredEntities> GetRequiredEntities(StandingCalculationResult result, CancellationToken cancellationToken)
    {
        RequiredEntities requiredEntities = new();
        requiredEntities.Members = await GetMemberEntities(result.StandingRows
            .Select(x => x.MemberId)
            .NotNull(), cancellationToken);
        requiredEntities.Teams = await GetTeamEntities(result.StandingRows
            .Select(x => x.TeamId)
            .NotNull(), cancellationToken);
        requiredEntities.ScoredResultRows = await GetScoredResultRowEntities(result.StandingRows
            .SelectMany(x => x.ResultRows)
            .Select(x => x.ScoredResultRowId)
            .NotNull(), cancellationToken);
        return requiredEntities;
    }

    private async Task<ICollection<MemberEntity>> GetMemberEntities(IEnumerable<long> memberIds, CancellationToken cancellationToken)
    {
        return await dbContext.Members
            .Where(x => memberIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    private async Task<ICollection<TeamEntity>> GetTeamEntities(IEnumerable<long> teamIds, CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .Where(x => teamIds.Contains(x.TeamId))
            .ToListAsync(cancellationToken);
    }

    private async Task<ICollection<ScoredResultRowEntity>> GetScoredResultRowEntities(IEnumerable<long> ids, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredResultRows
            .Where(x => ids.Contains(x.ScoredResultRowId))
            .ToListAsync(cancellationToken);
    }

    public async Task ClearStaleStandings(IEnumerable<long?> standingConfigIds, long eventId, CancellationToken cancellationToken = default)
    {
        var removeStandings = await dbContext.Standings
            .Where(x => x.EventId == eventId)
            .Where(x => standingConfigIds.Contains(x.StandingConfigId) == false)
            .ToListAsync(cancellationToken);
        foreach (var standing in removeStandings)
        {
            dbContext.Standings.Remove(standing);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
