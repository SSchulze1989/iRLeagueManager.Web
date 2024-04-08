using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class EventCalculationResultStore : DatabaseAccessBase, IEventCalculationResultStore
{
    public EventCalculationResultStore(LeagueDbContext dbContext) :
        base(dbContext)
    {
    }

    public async Task StoreCalculationResult(EventCalculationResult result, CancellationToken cancellationToken = default)
    {
        var eventResultEntity = await GetScoredEventResultEntity(result.EventId, result.ResultConfigId, cancellationToken);
        if (eventResultEntity == null)
        {
            eventResultEntity = await CreateScoredResultEntity(result.EventId, result.ResultConfigId, cancellationToken);
            dbContext.ScoredEventResults.Add(eventResultEntity);
        }
        var requiredEntities = await GetRequiredEntities(result, cancellationToken);
        await MapToEventResultEntity(result, eventResultEntity, requiredEntities, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return;
    }

    private async Task<ScoredEventResultEntity> MapToEventResultEntity(EventCalculationResult result, ScoredEventResultEntity entity,
        RequiredEntities requiredEntities, CancellationToken cancellationToken)
    {
        var members = await GetMemberEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .Select(x => x.MemberId)
            .OfType<long>(), cancellationToken);
        var teams = await GetTeamEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .Select(x => x.TeamId)
            .OfType<long>(), cancellationToken);
        entity.Name = result.Name;
        entity.ChampSeason = await dbContext.ChampSeasons
            .FirstOrDefaultAsync(x => x.ChampSeasonId == result.ChampSeasonId);
        entity.ScoredSessionResults = await MapToScoredSessionResults(result.SessionResults, entity.ScoredSessionResults,
            requiredEntities, cancellationToken);

        return entity;
    }

    private async Task<ICollection<ScoredSessionResultEntity>> MapToScoredSessionResults(IEnumerable<SessionCalculationResult> sessionResults,
        ICollection<ScoredSessionResultEntity> sessionResultEntities, RequiredEntities requiredEntities, CancellationToken cancellationToken)
    {
        var keepResults = new List<ScoredSessionResultEntity>();
        foreach (var sessionResult in sessionResults)
        {
            var sessionResultEntity = sessionResultEntities
                .Where(x => x.SessionResultId != 0)
                .FirstOrDefault(x => x.SessionNr == sessionResult.SessionNr);
            if (sessionResultEntity == null)
            {
                sessionResultEntity = await CreateScoredSessionResultEntity(sessionResult.ScoringId, cancellationToken);
                sessionResultEntities.Add(sessionResultEntity);
            }
            sessionResultEntity = MapToScoredSessionResultEntity(sessionResult, sessionResultEntity, requiredEntities);
            keepResults.Add(sessionResultEntity);
        }
        foreach (var removeResult in sessionResultEntities.Except(keepResults).ToList())
        {
            sessionResultEntities.Remove(removeResult);
        }
        return sessionResultEntities;
    }

    private ScoredSessionResultEntity MapToScoredSessionResultEntity(SessionCalculationResult result, ScoredSessionResultEntity entity,
        RequiredEntities requiredEntities)
    {
        entity.Name = result.Name;
        entity.SessionNr = result.SessionNr ?? 0;
        entity.CleanestDrivers = requiredEntities.Members.Where(x => result.CleanestDrivers.Contains(x.Id)).ToList();
        entity.FastestAvgLap = result.FastestAvgLap;
        entity.FastestAvgLapDriver = requiredEntities.Members.FirstOrDefault(x => x.Id == result.FastestQualyLapDriverMemberId);
        entity.FastestLap = result.FastestLap;
        entity.FastestLapDriver = requiredEntities.Members.FirstOrDefault(x => x.Id == result.FastestLapDriverMemberId);
        entity.FastestQualyLap = result.FastestQualyLap;
        entity.FastestQualyLapDriver = requiredEntities.Members.FirstOrDefault(x => x.Id == result.FastestQualyLapDriverMemberId);
        entity.HardChargers = requiredEntities.Members.Where(x => result.HardChargers.Contains(x.Id)).ToList();
        entity.ScoredResultRows = MapToScoredResultRows(result.ResultRows, entity.ScoredResultRows, requiredEntities);

        return entity;
    }

    private ICollection<ScoredResultRowEntity> MapToScoredResultRows(IEnumerable<ResultRowCalculationResult> resultRows,
        ICollection<ScoredResultRowEntity> rowEntities, RequiredEntities requiredEntities)
    {
        var keepRows = new List<ScoredResultRowEntity>();
        foreach (var row in resultRows)
        {
            ScoredResultRowEntity? rowEntity = default;
            if (row.MemberId != null)
            {
                rowEntity = rowEntities
                    .FirstOrDefault(x => x.MemberId == row.MemberId);
            }
            else if (row.TeamId != null)
            {
                rowEntity = rowEntities
                    .FirstOrDefault(x => x.TeamId == row.TeamId);
            }
            if (rowEntity == null)
            {
                rowEntity = new ScoredResultRowEntity();
                rowEntities.Add(rowEntity);
            }
            rowEntity = MaptoScoredResultRow(row, rowEntity, requiredEntities);
            keepRows.Add(rowEntity);
        }
        foreach (var removeRows in rowEntities.Except(keepRows).ToList())
        {
            rowEntities.Remove(removeRows);
        }
        return rowEntities;
    }

    private ScoredResultRowEntity MaptoScoredResultRow(ResultRowCalculationResult row, ScoredResultRowEntity rowEntity,
        RequiredEntities requiredEntities)
    {
        rowEntity.Member = requiredEntities.Members.FirstOrDefault(x => x.Id == row.MemberId);
        rowEntity.Team = requiredEntities.Teams.FirstOrDefault(x => x.TeamId == row.TeamId);
        rowEntity.AvgLapTime = row.AvgLapTime;
        rowEntity.BonusPoints = row.BonusPoints;
        rowEntity.Car = row.Car;
        rowEntity.CarClass = row.CarClass;
        rowEntity.CarId = row.CarId;
        rowEntity.CarNumber = row.CarNumber;
        rowEntity.ClassId = row.ClassId;
        rowEntity.ClubId = row.ClubId;
        rowEntity.ClubName = row.ClubName;
        rowEntity.CompletedLaps = row.CompletedLaps;
        rowEntity.CompletedPct = row.CompletedPct;
        rowEntity.Division = row.Division;
        rowEntity.FastestLapTime = row.FastestLapTime;
        rowEntity.FastLapNr = row.FastLapNr;
        rowEntity.FinalPosition = row.FinalPosition;
        rowEntity.FinalPositionChange = row.FinalPositionChange;
        rowEntity.FinishPosition = row.FinishPosition;
        rowEntity.Incidents = row.Incidents;
        rowEntity.Interval = row.Interval;
        rowEntity.LeadLaps = row.LeadLaps;
        rowEntity.License = row.License;
        rowEntity.NewCpi = row.NewCpi;
        rowEntity.NewIRating = row.NewIrating;
        rowEntity.NewLicenseLevel = row.NewLicenseLevel;
        rowEntity.NewSafetyRating = row.NewSafetyRating;
        rowEntity.OldCpi = row.OldCpi;
        rowEntity.OldIRating = row.OldIrating;
        rowEntity.OldLicenseLevel = row.OldLicenseLevel;
        rowEntity.OldSafetyRating = row.OldSafetyRating;
        rowEntity.PenaltyPoints = row.PenaltyPoints;
        rowEntity.PointsEligible = row.PointsEligible;
        rowEntity.PositionChange = row.PositionChange;
        rowEntity.QualifyingTime = row.QualifyingTime;
        rowEntity.RacePoints = row.RacePoints;
        rowEntity.ReviewPenalties = MapToReviewPenaltyList(row, rowEntity.ReviewPenalties, requiredEntities);
        rowEntity.SeasonStartIRating = row.SeasonStartIrating;
        rowEntity.StartPosition = row.StartPosition;
        rowEntity.Status = row.Status;
        rowEntity.TotalPoints = row.TotalPoints;
        rowEntity.TeamResultRows = requiredEntities.ScoredResultRows
            .Where(x => row.ScoredMemberResultRowIds.Contains(x.ScoredResultRowId)).ToList();
        return rowEntity;
    }

    private ICollection<ReviewPenaltyEntity> MapToReviewPenaltyList(ResultRowCalculationResult row, ICollection<ReviewPenaltyEntity> penaltyEntities,
        RequiredEntities requiredEntities)
    {
        foreach (var penalty in row.ReviewPenalties)
        {
            var penaltyEntity = penaltyEntities
                .Where(x => x.ReviewVoteId == penalty.ReviewVoteId)
                .FirstOrDefault();
            if (penaltyEntity is null)
            {
                var review = requiredEntities.Reviews
                    .FirstOrDefault(x => x.ReviewId == penalty.ReviewId);
                var vote = review?.AcceptedReviewVotes
                    .FirstOrDefault(x => x.ReviewVoteId == penalty.ReviewVoteId);
                if (review is null || vote is null)
                {
                    continue;
                }
                penaltyEntity = new ReviewPenaltyEntity()
                {
                    LeagueId = review.LeagueId,
                    Review = review,
                    ReviewVote = vote,
                    Value = new(),
                };
                penaltyEntities.Add(penaltyEntity);
            }
            penaltyEntity.Value.Type = PenaltyType.Points;
            penaltyEntity.Value.Points = penalty.PenaltyPoints;
        }
        return penaltyEntities;
    }

    private async Task<ScoredSessionResultEntity> CreateScoredSessionResultEntity(long? scoringId, CancellationToken cancellationToken)
    {
        var scoring = await dbContext.Scorings
            .FirstOrDefaultAsync(x => x.ScoringId == scoringId, cancellationToken);
        var sessionResult = new ScoredSessionResultEntity()
        {
            Scoring = scoring,
        };
        return sessionResult;
    }

    private async Task<ScoredEventResultEntity> CreateScoredResultEntity(long eventId, long? resultConfigId, CancellationToken cancellationToken)
    {
        var @event = await dbContext.Events
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken)
            ?? throw new InvalidOperationException($"No event with id:{eventId} exists");
        var resultConfig = await dbContext.ResultConfigurations
            .FirstOrDefaultAsync(x => x.ResultConfigId == resultConfigId, cancellationToken);
        var eventResult = new ScoredEventResultEntity()
        {
            Event = @event,
            ResultConfig = resultConfig,
        };
        return eventResult;
    }

    private async Task<ScoredEventResultEntity?> GetScoredEventResultEntity(long eventId, long? resultConfigId, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredEventResults
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.ScoredResultRows)
                    .ThenInclude(x => x.Member)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.ScoredResultRows)
                    .ThenInclude(x => x.TeamResultRows)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.ScoredResultRows)
                    .ThenInclude(x => x.ReviewPenalties)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.HardChargers)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.CleanestDrivers)
            .Where(x => x.EventId == eventId)
            .Where(x => x.ResultConfigId == resultConfigId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<RequiredEntities> GetRequiredEntities(EventCalculationResult result, CancellationToken cancellationToken)
    {
        RequiredEntities requiredEntities = new();
        requiredEntities.Members = await GetMemberEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .Select(x => x.MemberId)
            .NotNull(), cancellationToken);
        requiredEntities.Teams = await GetTeamEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .Select(x => x.TeamId)
            .NotNull(), cancellationToken);
        requiredEntities.ScoredResultRows = await GetScoredResultRowEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .SelectMany(x => x.ScoredMemberResultRowIds), cancellationToken);
        requiredEntities.Reviews = await GetReviewEntities(result.SessionResults
            .SelectMany(x => x.ResultRows)
            .SelectMany(x => x.ReviewPenalties)
            .Select(x => (x.ReviewId))
            .NotNull(), cancellationToken);
        return requiredEntities;
    }

    private async Task<ICollection<IncidentReviewEntity>> GetReviewEntities(IEnumerable<long> reviewIds, CancellationToken cancellationToken)
    {
        return await dbContext.IncidentReviews
            .Include(x => x.AcceptedReviewVotes)
            .Where(x => reviewIds.Contains(x.ReviewId))
            .ToListAsync(cancellationToken);
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
}
