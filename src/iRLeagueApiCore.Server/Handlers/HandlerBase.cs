using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Diagnostics.CodeAnalysis;

namespace iRLeagueApiCore.Server.Handlers;

public abstract class HandlerBase<THandler, TRequest>
{
    protected ILogger<THandler> _logger;
    protected LeagueDbContext dbContext;
    protected IEnumerable<IValidator<TRequest>> validators;

    protected HandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        this.dbContext = dbContext;
        this.validators = validators;
    }

    protected virtual async Task<LeagueEntity?> GetCurrentLeagueEntityAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Leagues
            .FirstOrDefaultAsync(x => x.Id == dbContext.LeagueProvider.LeagueId, cancellationToken);
    }

    protected virtual async Task<LeagueEntity?> GetLeagueEntityAsync(long leagueId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Leagues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == leagueId, cancellationToken);
    }

    protected virtual async Task<ScheduleEntity?> GetScheduleEntityAsync(long? scheduleId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Schedules
            .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId, cancellationToken);
    }

    protected virtual async Task<ScoringEntity?> GetScoringEntityAsync(long? scoringId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Scorings
            .FirstOrDefaultAsync(x => x.ScoringId == scoringId, cancellationToken);
    }

    protected virtual async Task<SeasonEntity?> GetSeasonEntityAsync(long? seasonId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Seasons
            .FirstOrDefaultAsync(x => x.SeasonId == seasonId, cancellationToken);
    }

    protected virtual async Task<TrackConfigEntity?> GetTrackConfigEntityAsync(long? trackConfigId, CancellationToken cancellationToken = default)
    {
        return await dbContext.TrackConfigs
            .FirstOrDefaultAsync(x => x.TrackId == trackConfigId, cancellationToken);
    }

    protected virtual async Task<MemberEntity?> GetMemberEntityAsync(long? memberId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Members
            .FirstOrDefaultAsync(x => x.Id == memberId, cancellationToken);
    }

    protected virtual async Task<TeamEntity?> GetTeamEntityAsync(long? teamId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Teams
            .FirstOrDefaultAsync(x => x.TeamId == teamId, cancellationToken);
    }

    protected virtual async Task<VoteCategoryEntity?> GetVoteCategoryEntityAsync(long leagueId, long? voteCategoryId, CancellationToken cancellationToken = default)
    {
        return await dbContext.VoteCategories
            .FirstOrDefaultAsync(x => x.CatId == voteCategoryId, cancellationToken);
    }

    protected virtual async Task<ICollection<MemberEntity>> GetMemberListAsync(IEnumerable<long> memberIds, CancellationToken cancellationToken = default)
    {
        return await dbContext.Members
            .Where(x => memberIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    protected virtual async Task<ICollection<TeamEntity>> GetTeamListAsync(IEnumerable<long> teamIds, CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .Where(x => teamIds.Contains(x.TeamId))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns a utc date time from the provided datetime without performin a conversion
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(dateTime))]
    protected static DateTime? TreatAsUTCDateTime(DateTime? dateTime)
    {
        if (dateTime is null)
        {
            return null;
        }
        return new DateTime(dateTime!.Value.Ticks, DateTimeKind.Utc);
    }

    protected virtual T CreateVersionEntity<T>(LeagueUser user, T target) where T : IVersionEntity
    {
        target.CreatedOn = DateTime.UtcNow;
        target.CreatedByUserId = user.Id;
        target.CreatedByUserName = user.Name;
        target.Version = 0;
        return target;
    }

    protected virtual T UpdateVersionEntity<T>(LeagueUser user, T target) where T : IVersionEntity
    {
        target.LastModifiedOn = DateTime.UtcNow;
        target.LastModifiedByUserId = user.Id;
        target.LastModifiedByUserName = user.Name;
        target.Version++;
        return target;
    }

    protected async Task<IDictionary<long, int>> GetMemberSeasonStartIratingAsync(long seasonId, CancellationToken cancellationToken)
    {
        return (await dbContext.ResultRows
            .Where(x => x.SubResult.Result.Event.Schedule.SeasonId == seasonId)
            .OrderBy(x => x.SubResult.Result.Event.Date)
            .Select(x => new { x.MemberId, x.OldIRating })
            .ToListAsync(cancellationToken))
            .DistinctBy(x => x.MemberId)
            .ToDictionary(k => k.MemberId, k => k.OldIRating);
    }

    protected static TimeSpan ParseTime(int value) => value < 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(value / 10000D);

    protected static TimeSpan ParseTime(long value) => value < 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(value / 10000D);

    protected static TimeSpan ParseInterval(int value, int completedLaps, int sessionLaps)
    {
        return ParseInterval(TimeSpan.FromSeconds(value / 10000D), completedLaps, sessionLaps);
    }

    protected static TimeSpan ParseInterval(TimeSpan value, int completedLaps, int sessionLaps)
    {
        if (value > TimeSpan.Zero)
        {
            return value;
        }
        return TimeSpan.FromDays(sessionLaps - completedLaps);
    }

    protected static (string, string) GetFirstnameLastname(string name)
    {
        var parts = name.Split(' ', 2);
        var fullName = (parts[0], parts.ElementAt(1) ?? string.Empty);
        return fullName;
    }

    protected IEnumerable<SessionResultEntity> MapToSubSessions(IDictionary<int, SessionResultEntity> subResults, EventEntity @event,
        ICollection<(int resultNr, int sessionNr)> map)
    {
        var mappedResults = new List<SessionResultEntity>();
        foreach ((var resultNr, var sessionNr) in map)
        {
            if (subResults.ContainsKey(resultNr) == false)
            {
                throw new InvalidOperationException($"Error while trying to map subResult Nr.{resultNr} to subSession Nr.{sessionNr}: no result with this subResultNr exists");
            }
            var sessionResult = subResults[resultNr];
            var session = @event.Sessions
                .SingleOrDefault(x => x.SessionNr == sessionNr)
                ?? throw new InvalidOperationException($"Error while trying to map subResult Nr.{resultNr} to subSession Nr.{sessionNr}: no subSession with this subSessionNr exists");
            sessionResult.Session = session;
            session.SessionResult = sessionResult;
            mappedResults.Add(sessionResult);
        }
        return mappedResults;
    }

    protected async Task<ICollection<FilterOptionEntity>> MapToFilterOptionListAsync(LeagueUser user, IEnumerable<ResultFilterModel> filterModels,
        ICollection<FilterOptionEntity> filterEntities, CancellationToken cancellationToken)
    {
        foreach (var filterModel in filterModels)
        {
            var filterOptionEntity = filterModel.FilterOptionId == 0 ? null : filterEntities
                .FirstOrDefault(x => x.FilterOptionId == filterModel.FilterOptionId);
            if (filterOptionEntity is null)
            {
                filterOptionEntity = CreateVersionEntity(user, new FilterOptionEntity());
                filterEntities.Add(filterOptionEntity);
                filterOptionEntity.LeagueId = dbContext.LeagueProvider.LeagueId;
            }
            await MapToFilterOptionEntityAsync(user, filterModel, filterOptionEntity, cancellationToken);
        }
        var deleteFilterEntities = filterEntities
            .Where(x => filterModels.Any(y => y.FilterOptionId == x.FilterOptionId) == false);
        foreach (var deleteFilterEntity in deleteFilterEntities)
        {
            filterEntities.Remove(deleteFilterEntity);
        }
        return filterEntities;
    }

    protected Task<FilterOptionEntity> MapToFilterOptionEntityAsync(LeagueUser user, ResultFilterModel filterModel, FilterOptionEntity filterOptionEntity,
        CancellationToken cancellationToken)
    {

        filterOptionEntity.Conditions = new[]
        {
            new FilterConditionModel()
            {
                Comparator = filterModel.Condition.Comparator,
                FilterType = filterModel.Condition.FilterType,
                ColumnPropertyName = filterModel.Condition.ColumnPropertyName,
                FilterValues = filterModel.Condition.FilterValues,
                Action = filterModel.Condition.Action
            }
        }.ToList();
        UpdateVersionEntity(user, filterOptionEntity);
        return Task.FromResult(filterOptionEntity);
    }
}
