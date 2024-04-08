using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class StandingCalculationDataProvider : DatabaseAccessBase, IStandingCalculationDataProvider
{
    public StandingCalculationDataProvider(LeagueDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<StandingCalculationData?> GetData(StandingCalculationConfiguration config, CancellationToken cancellationToken = default)
    {
        // 1. Get season and event entity from config
        var season = await GetSeasonEntityAsync(config.SeasonId, cancellationToken)
            ?? throw new InvalidOperationException($"No season with id {config.SeasonId} found");
        var currentEvent = await GetCurrentEventEntityAsync(config.EventId, cancellationToken)
            ?? throw new InvalidOperationException($"No event with id {config.EventId} found");
        // 2. Load all results from events prior to configured event
        var resultConfigIds = config.ResultConfigs.Count() == 0 ? new[] { default(long?) } : config.ResultConfigs.Cast<long?>();
        var previousResults = await GetPreviousResultsAsync(season.SeasonId, resultConfigIds, currentEvent.Date, cancellationToken);
        // 3. Load results from latest event
        var currentResults = await GetCurrentEventResultAsync(currentEvent.EventId, resultConfigIds, cancellationToken);
        if (currentResults is null && previousResults.None())
        {
            return null;
        }

        // 4. return data
        var data = new StandingCalculationData()
        {
            LeagueId = config.LeagueId,
            EventId = config.EventId,
            SeasonId = config.SeasonId,
            PreviousEventResults = previousResults.ToList(),
            CurrentEventResult = currentResults ?? new() { LeagueId = config.LeagueId, EventId = currentEvent.EventId },
        };

        return data;
    }

    private async Task<SeasonEntity?> GetSeasonEntityAsync(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.Seasons
            .FirstOrDefaultAsync(x => x.SeasonId == seasonId, cancellationToken);
    }

    private async Task<EventEntity?> GetCurrentEventEntityAsync(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
    }

    private async Task<IEnumerable<EventCalculationResult>> GetPreviousResultsAsync(long seasonId, IEnumerable<long?> resultConfigIds, DateTime? date, CancellationToken cancellationToken)
    {
        if (date is null)
        {
            return Array.Empty<EventCalculationResult>();
        }

        return await dbContext.ScoredEventResults
            .Where(x => x.Event.Schedule.SeasonId == seasonId)
            .Where(x => resultConfigIds.Contains(x.ResultConfigId))
            .OrderBy(x => x.Event.Date)
            .Where(x => x.Event.Date < date.Value)
            .Select(MapToEventResultCalculationResultExpression)
            .ToListAsync(cancellationToken);
    }

    private async Task<EventCalculationResult?> GetCurrentEventResultAsync(long eventId, IEnumerable<long?> resultConfigIds, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredEventResults
            .Where(x => x.Event.EventId == eventId)
            .Where(x => resultConfigIds.Contains(x.ResultConfigId))
            .Select(MapToEventResultCalculationResultExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static Expression<Func<ScoredEventResultEntity, EventCalculationResult>> MapToEventResultCalculationResultExpression => eventResult => new EventCalculationResult()
    {
        LeagueId = eventResult.LeagueId,
        EventId = eventResult.EventId,
        Name = eventResult.Name,
        ResultConfigId = eventResult.ResultConfigId,
        ResultId = eventResult.ResultId,
        SessionResults = eventResult.ScoredSessionResults.Select(sessionResult => new SessionCalculationResult()
        {
            LeagueId = sessionResult.LeagueId,
            SessionNr = sessionResult.SessionNr,
            FastestAvgLap = sessionResult.FastestAvgLap,
            FastestAvgLapDriverMemberId = sessionResult.FastestAvgLapDriverMemberId,
            FastestLap = sessionResult.FastestLap,
            FastestLapDriverMemberId = sessionResult.FastestLapDriverMemberId,
            FastestQualyLap = sessionResult.FastestQualyLap,
            FastestQualyLapDriverMemberId = sessionResult.FastestQualyLapDriverMemberId,
            CleanestDrivers = sessionResult.CleanestDrivers.Select(x => x.Id).ToList(),
            HardChargers = sessionResult.HardChargers.Select(x => x.Id).ToList(),
            Name = sessionResult.Name,
            ScoringId = sessionResult.ScoringId,
            SessionResultId = sessionResult.SessionResultId,
            ResultRows = sessionResult.ScoredResultRows.Select(row => new ResultRowCalculationResult()
            {
                ScoredResultRowId = row.ScoredResultRowId,
                MemberId = row.MemberId,
                Firstname = row.Member == null ? string.Empty : row.Member.Firstname,
                Lastname = row.Member == null ? string.Empty : row.Member.Lastname,
                TeamId = row.TeamId,
                TeamName = row.Team == null ? string.Empty : row.Team.Name,
                AvgLapTime = row.AvgLapTime,
                Car = row.Car,
                CarClass = row.CarClass,
                CarId = row.CarId,
                CarNumber = row.CarNumber,
                ClassId = row.ClassId,
                CompletedLaps = row.CompletedLaps,
                CompletedPct = row.CompletedPct,
                FastestLapTime = row.FastestLapTime,
                FastLapNr = row.FastLapNr,
                FinishPosition = row.FinishPosition,
                Division = row.Division,
                Incidents = row.Incidents,
                Interval = row.Interval,
                LeadLaps = row.LeadLaps,
                License = row.License,
                NewIrating = row.NewIRating,
                NewLicenseLevel = row.NewLicenseLevel,
                NewSafetyRating = row.NewSafetyRating,
                OldIrating = row.OldIRating,
                OldLicenseLevel = row.OldLicenseLevel,
                OldSafetyRating = row.OldSafetyRating,
                QualifyingTime = row.QualifyingTime,
                PositionChange = row.PositionChange,
                RacePoints = row.RacePoints,
                SeasonStartIrating = row.SeasonStartIRating,
                StartPosition = row.StartPosition,
                Status = row.Status,
                TotalPoints = row.RacePoints,
                BonusPoints = row.BonusPoints,
                ClubName = row.ClubName,
                ClubId = row.ClubId,
                FinalPosition = row.FinalPosition,
                FinalPositionChange = row.FinalPositionChange,
                NewCpi = row.NewCpi,
                OldCpi = row.OldCpi,
                PenaltyPoints = row.PenaltyPoints,
                ScoredMemberResultRowIds = row.TeamResultRows.Select(x => x.ScoredResultRowId).ToList(),
                PointsEligible = row.PointsEligible,
            })
        })
    };
}
