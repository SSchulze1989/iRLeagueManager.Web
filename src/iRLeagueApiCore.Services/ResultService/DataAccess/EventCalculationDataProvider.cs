using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Relational;
using System.Diagnostics.Eventing.Reader;
using System.Threading;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class EventCalculationDataProvider : DatabaseAccessBase, IEventCalculationDataProvider
{
    public EventCalculationDataProvider(LeagueDbContext dbContext) :
        base(dbContext)
    {
    }

    public async Task<EventCalculationData?> GetData(EventCalculationConfiguration config, CancellationToken cancellationToken = default)
    {
        var data = await GetDataFromConfiguration(config, cancellationToken);
        if (data is null)
        {
            return data;
        }

        data = await AssociatePenalties(config, data, cancellationToken);
        // Fill Qualy lap
        data = FillQualyLap(config, data);
        data = FillFastestLapTimes(data);
        return data;
    }

    public async Task<EventCalculationData?> GetDataFromConfiguration(EventCalculationConfiguration config, CancellationToken cancellationToken = default)
    {
        if (config.SourceResultConfigId is not null)
        {
            return await dbContext.ScoredEventResults
                .Where(x => x.ResultConfigId == config.SourceResultConfigId)
                .Select(ScoredMapToEventResultCalculationDataExpression)
                .FirstOrDefaultAsync(x => x.EventId == config.EventId, cancellationToken);
        }

        return await dbContext.EventResults
            .Select(MapToEventResultCalculationDataExpression)
            .FirstOrDefaultAsync(x => x.EventId == config.EventId, cancellationToken);
    }

    /// <summary>
    /// Fill qualy lap time for first race session after qualifying
    /// </summary>
    /// <param name="config"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static EventCalculationData FillQualyLap(EventCalculationConfiguration config, EventCalculationData data)
    {
        // find driver qualy laps
        var qualySessionNr = config.SessionResultConfigurations.FirstOrDefault(x => x.SessionType == Common.Enums.SessionType.Qualifying)?.SessionNr;
        var qualySessionData = data.SessionResults.FirstOrDefault(x => x.SessionNr == qualySessionNr);
        if (qualySessionData is null)
        {
            return data;
        }

        // find driver qualy laps
        var driverQualyLaps = qualySessionData.ResultRows.Select(x => new { x.MemberId, x.QualifyingTime });

        // fill qualifying time for all sessions
        var firstRaceSessionNr = config.SessionResultConfigurations
            .Where(x => x.SessionNr > qualySessionNr)
            .Where(x => x.SessionType == Common.Enums.SessionType.Race)
            .FirstOrDefault()?.SessionNr;
        var firstRaceSession = data.SessionResults.FirstOrDefault(x => x.SessionNr == firstRaceSessionNr);
        if (firstRaceSession is null)
        {
            return data;
        }

        foreach(var row in firstRaceSession.ResultRows)
        {
            row.QualifyingTime = driverQualyLaps.FirstOrDefault(x => x.MemberId == row.MemberId)?.QualifyingTime ?? TimeSpan.Zero;
        }

        return data;
    }

    private EventCalculationData FillFastestLapTimes(EventCalculationData data)
    {
        foreach (var sessionResult in data.SessionResults)
        {
            sessionResult.FastestLap = sessionResult.ResultRows
                .Select(x => x.FastestLapTime)
                .Where(LapIsValid)
                .OrderBy(x => x)
                .FirstOrDefault();
            sessionResult.FastestQualyLap = sessionResult.ResultRows
                .Select(x => x.QualifyingTime)
                .Where(LapIsValid)
                .OrderBy(x => x)
                .FirstOrDefault();
            sessionResult.FastestAvgLap = sessionResult.ResultRows
                .Select(x => x.AvgLapTime)
                .Where(LapIsValid)
                .OrderBy(x => x)
                .FirstOrDefault();
        }
        return data;
    }

    private async Task<EventCalculationData> AssociatePenalties(EventCalculationConfiguration config, EventCalculationData data, CancellationToken cancellationToken)
    {
        // get existing scoredresultrows
        var scoredResultRows = await dbContext.ScoredResultRows
            .Include(x => x.ScoredSessionResult)
            .Include(x => x.AddPenalties)
            .Where(x => x.ScoredSessionResult.ResultId == config.ResultId)
            .ToListAsync(cancellationToken);
        if (scoredResultRows.None())
        {
            return data;
        }

        data.AddPenalties = scoredResultRows
            .SelectMany(row => row.AddPenalties
                .Select(x => new AddPenaltyCalculationData()
                {
                    SessionNr = row.ScoredSessionResult.SessionNr,
                    MemberId = row.MemberId,
                    TeamId = row.TeamId,
                    Type = x.Value.Type,
                    Points = x.Value.Points,
                    Positions = x.Value.Positions,
                    Time = x.Value.Time,
                }));
        return data;
    }

    private static bool LapIsValid(TimeSpan lap)
    {
        return lap > TimeSpan.Zero;
    }

    private static Expression<Func<EventResultEntity, EventCalculationData>> MapToEventResultCalculationDataExpression => eventResult => new EventCalculationData()
    {
        LeagueId = eventResult.LeagueId,
        EventId = eventResult.EventId,
        SessionResults = eventResult.SessionResults
            .Select(sessionResult => new SessionCalculationData()
            {
                LeagueId = sessionResult.LeagueId,
                SessionId = sessionResult.SessionId,
                SessionNr = sessionResult.Session.SessionNr,
                AcceptedReviewVotes = sessionResult.Session.IncidentReviews
                    .SelectMany(review => review.AcceptedReviewVotes)
                    .Select(vote => new AcceptedReviewVoteCalculationData()
                    {
                        DefaultPenalty = vote.VoteCategory == null ? 0 : vote.VoteCategory.DefaultPenalty,
                        MemberAtFaultId = vote.MemberAtFaultId,
                        TeamAtFaultId = vote.TeamAtFaultId,
                        ReviewId = vote.ReviewId,
                        VoteCategoryId = vote.VoteCategoryId,
                        ReviewVoteId = vote.ReviewVoteId,
                    }),
                Sof = sessionResult.IRSimSessionDetails == null ? 0 : sessionResult.IRSimSessionDetails.EventStrengthOfField,
                ResultRows = sessionResult.ResultRows.Select(row => new ResultRowCalculationData()
                {
                    ScoredResultRowId = null,
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
                    ClubId = row.ClubId,
                    ClubName = row.ClubName,
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
                    NewCpi = row.NewCpi,
                    NewLicenseLevel = row.NewLicenseLevel,
                    NewSafetyRating = row.NewSafetyRating,
                    OldIrating = row.OldIRating,
                    OldCpi = row.OldCpi,
                    OldLicenseLevel = row.OldLicenseLevel,
                    OldSafetyRating = row.OldSafetyRating,
                    QualifyingTime = row.QualifyingTime,
                    PositionChange = row.PositionChange,
                    RacePoints = row.RacePoints,
                    SeasonStartIrating = row.SeasonStartIRating,
                    StartPosition = row.StartPosition,
                    Status = row.Status,
                    TotalPoints = row.RacePoints,
                    PointsEligible = row.PointsEligible,
                })
            })
    };

    private static Expression<Func<ScoredEventResultEntity, EventCalculationData>> ScoredMapToEventResultCalculationDataExpression => eventResult => new EventCalculationData()
    {
        LeagueId = eventResult.LeagueId,
        EventId = eventResult.EventId,
        SessionResults = eventResult.ScoredSessionResults.Select(sessionResult => new SessionCalculationData()
        {
            LeagueId = sessionResult.LeagueId,
            SessionNr = sessionResult.SessionNr,
            ResultRows = sessionResult.ScoredResultRows.Select(row => new ResultRowCalculationData()
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
                BonusPoints = row.BonusPoints,
                PenaltyPoints = row.PenaltyPoints,
                SeasonStartIrating = row.SeasonStartIRating,
                StartPosition = row.StartPosition,
                Status = row.Status,
                TotalPoints = row.RacePoints,
                PointsEligible = row.PointsEligible,
            })
        })
    };
}
