using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Results;

public class ResultHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ResultHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    protected async Task<IEnumerable<EventResultModel>> MapToGetResultModelsFromEventAsync(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredEventResults
            .Where(x => x.EventId == eventId)
            .Select(MapToEventResultModelExpression)
            .ToListAsync(cancellationToken);
    }

    public Expression<Func<ScoredEventResultEntity, EventResultModel>> MapToEventResultModelExpression => result => new EventResultModel()
    {
        EventId = result.EventId,
        LeagueId = result.LeagueId,
        EventName = result.Event.Name,
        DisplayName = result.Name,
        ResultId = result.ResultId,
        SeasonId = result.Event.Schedule.SeasonId,
        Date = result.Event.Date.GetValueOrDefault(),
        TrackId = result.Event.TrackId.GetValueOrDefault(),
        TrackName = result.Event.Track.TrackGroup.TrackName,
        ConfigName = result.Event.Track.ConfigName,
        StrengthOfField = result.Event.SimSessionDetails.Any() ? result.Event.SimSessionDetails.First().EventStrengthOfField : 0,
        SessionResults = result.ScoredSessionResults.Select(sessionResult => new ResultModel()
        {
            LeagueId = sessionResult.LeagueId,
            ScoringId = sessionResult.ScoringId,
            SessionResultId = sessionResult.SessionResultId,
            SessionName = sessionResult.Name,
            SessionNr = sessionResult.SessionNr,
            ResultRows = sessionResult.ScoredResultRows
                .OrderBy(x => x.FinalPosition)
                .Select(row => new ResultRowModel()
            {
                ScoredResultRowId = row.ScoredResultRowId,
                MemberId = row.MemberId,
                Interval = new Interval(row.Interval),
                FastestLapTime = row.FastestLapTime,
                AvgLapTime = row.AvgLapTime,
                Firstname = (row.Member != null) ? row.Member.Firstname : string.Empty,
                Lastname = (row.Member != null) ? row.Member.Lastname : string.Empty,
                TeamName = (row.Team != null) ? row.Team.Name : string.Empty,
                StartPosition = row.StartPosition,
                FinishPosition = row.FinishPosition,
                FinalPosition = row.FinalPosition,
                RacePoints = row.RacePoints,
                PenaltyPoints = row.PenaltyPoints,
                BonusPoints = row.BonusPoints,
                TotalPoints = row.TotalPoints,
                Car = row.Car,
                CarClass = row.CarClass,
                CarId = row.CarId,
                CarNumber = row.CarNumber,
                ClassId = row.ClassId,
                ClubId = row.ClubId,
                ClubName = row.ClubName,
                CompletedLaps = row.CompletedLaps,
                CompletedPct = row.CompletedPct,
                Division = row.Division,
                FastLapNr = row.FastLapNr,
                FinalPositionChange = row.FinalPositionChange,
                Incidents = row.Incidents,
                LeadLaps = row.LeadLaps,
                License = row.License,
                NewIrating = row.NewIRating,
                NewLicenseLevel = row.NewLicenseLevel,
                NewSafetyRating = row.NewSafetyRating,
                OldIrating = row.OldIRating,
                OldLicenseLevel = row.OldLicenseLevel,
                OldSafetyRating = row.OldSafetyRating,
                PositionChange = row.PositionChange,
                QualifyingTime = row.QualifyingTime,
                SeasonStartIrating = row.SeasonStartIRating,
                Status = (RaceStatus)row.Status,
                TeamId = row.TeamId,
                TeamColor = (row.Team != null) ? row.Team.TeamColor : string.Empty,
            }),
            FastestLapTime = sessionResult.FastestLap,
            FastestLapDriver = sessionResult.FastestLapDriver == null ? null : new()
            {
                FirstName = sessionResult.FastestLapDriver.Firstname,
                LastName = sessionResult.FastestLapDriver.Lastname,
                MemberId = sessionResult.FastestLapDriver.Id,
            },
            PoleLapTime = sessionResult.FastestQualyLap,
            PoleLapDriver = sessionResult.FastestQualyLapDriver == null ? null : new()
            {
                FirstName = sessionResult.FastestQualyLapDriver.Firstname,
                LastName = sessionResult.FastestQualyLapDriver.Lastname,
                MemberId = sessionResult.FastestQualyLapDriver.Id,
            },
            CleanestDrivers = sessionResult.CleanestDrivers.Select(member => new MemberInfoModel()
            {
                FirstName = member.Firstname,
                LastName = member.Lastname,
                MemberId = member.Id,
            }).ToList(),
            CreatedOn = TreatAsUTCDateTime(sessionResult.CreatedOn),
            LastModifiedOn = TreatAsUTCDateTime(sessionResult.LastModifiedOn),
        }),
    };
}
