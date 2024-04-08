using Aydsko.iRacingData;
using Aydsko.iRacingData.Results;
using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Excecution;
using System.Net;
using System.Transactions;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record FetchResultsFromIRacingAPIRequest(long EventId, int IRSubsessionId) : IRequest<bool>;

public class FetchResultsFromIRacingAPIHandler : HandlerBase<FetchResultsFromIRacingAPIHandler, FetchResultsFromIRacingAPIRequest>, 
    IRequestHandler<FetchResultsFromIRacingAPIRequest, bool>
{
    private readonly ICredentials credentials;
    private readonly IDataClient iRDataClient;
    private IDictionary<long, int> SeasonStartIratings;
    private readonly IResultCalculationQueue calculationQueue;

    public FetchResultsFromIRacingAPIHandler(ILogger<FetchResultsFromIRacingAPIHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<FetchResultsFromIRacingAPIRequest>> validators, ICredentials credentials, IDataClient iRDataClient, IResultCalculationQueue calculationQueue)
        : base(logger, dbContext, validators)
    {
        this.credentials = credentials;
        this.iRDataClient = iRDataClient;
        SeasonStartIratings = new Dictionary<long, int>();
        this.calculationQueue = calculationQueue;
    }

    public async Task<bool> Handle(FetchResultsFromIRacingAPIRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var @event = await GetResultEventEntityAsync(request.EventId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        SeasonStartIratings = await GetMemberSeasonStartIratingAsync(@event.Schedule.SeasonId, cancellationToken);
        var credential = credentials.GetCredential(new Uri("https://members-ng.iracing.com/auth"), "Token")
            ?? throw new InvalidOperationException("Could not find credentials for iracing service - check configuration");
        iRDataClient.UseUsernameAndPassword(credential.UserName, credential.Password);
        var resultResponse = await iRDataClient.GetSubSessionResultAsync(request.IRSubsessionId, true, cancellationToken);
        var resultData = resultResponse.Data;
        using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            if (@event.EventResult is not null)
            {
                dbContext.Remove(@event.EventResult);
            }
            @event.SimSessionDetails.Clear();
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await ReadResultsAsync(resultData, @event, cancellationToken);
            @event.EventResult = result;
            await dbContext.SaveChangesAsync(cancellationToken);
            tx.Complete();
        }
        // Queue result calculation for this event
        await calculationQueue.QueueEventResultAsync(@event.EventId);

        return true;
    }

    private async Task<EventResultEntity> ReadResultsAsync(SubSessionResult data, EventEntity @event,
        CancellationToken cancellationToken)
    {
        var map = CreateSessionMapping(data.SessionResults, @event);
        // create entities
        var result = @event.EventResult ?? new EventResultEntity();
        result.Event = @event;
        var details = ReadDetails(data);
        details.Event = @event;
        IDictionary<int, SessionResultEntity> sessionResults = new Dictionary<int, SessionResultEntity>();
        foreach (var sessionResultData in data.SessionResults)
        {
            var sessionResult = await ReadSessionResultsAsync(@event.LeagueId, data, sessionResultData, details, cancellationToken);
            sessionResults.Add(sessionResult);
        }
        var mappedSessionResults = MapToSubSessions(sessionResults, @event, map);
        foreach (var subResult in mappedSessionResults)
        {
            result.SessionResults.Add(subResult);
        }
        return result;
    }

    private static ICollection<(int resultNr, int sessionNr)> CreateSessionMapping(IEnumerable<SessionResults> sessionResults, EventEntity @event)
    {
        var map = new List<(int resultNr, int sessionNr)>();

        var practiceSessionTypes = new[] { SimSessionType.OpenPractice }.Cast<int>();
        var practiceSession = @event.Sessions
            .FirstOrDefault(x => x.SessionType == SessionType.Practice);
        var practiceResult = sessionResults
            .FirstOrDefault(x => practiceSessionTypes.Contains(x.SimSessionType) && x.SimSessionName == "PRACTICE");
        if (practiceSession is not null && practiceResult is not null)
        {
            map.Add((practiceResult.SimSessionNumber, practiceSession.SessionNr));
        }

        var qualySessionTypes = new[] { SimSessionType.LoneQualifying, SimSessionType.OpenQualifying }.Cast<int>();
        var qualySession = @event.Sessions
            .FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
        var qualyResult = sessionResults
            .FirstOrDefault(x => qualySessionTypes.Contains(x.SimSessionType));
        if (qualySession is not null && qualyResult is not null)
        {
            map.Add((qualyResult.SimSessionNumber, qualySession.SessionNr));
        }

        var raceSessionTypes = new[] { SimSessionType.Race }.Cast<int>();
        var raceSessions = @event.Sessions
            .OrderBy(x => x.SessionNr)
            .Where(x => x.SessionType == SessionType.Race);
        var raceResults = sessionResults
            .Where(x => raceSessionTypes.Contains(x.SimSessionType));
        foreach ((var session, var result) in raceSessions.Zip(raceResults))
        {
            if (session is not null && result is not null)
            {
                map.Add((result.SimSessionNumber, session.SessionNr));
            }
        }

        return map;
    }

    private async Task<LeagueMemberEntity> GetOrCreateMemberAsync(long leagueId, Result row, CancellationToken cancellationToken)
    {
        var leagueMember = await dbContext.LeagueMembers
            .Include(x => x.Team)
            .Include(x => x.Member)
            .Where(x => x.LeagueId == leagueId)
            .Where(x => x.Member.IRacingId == row.CustomerId.ToString())
            .SingleOrDefaultAsync(cancellationToken)
            ?? dbContext.LeagueMembers.Local
            .SingleOrDefault(x => x.Member.IRacingId == row.CustomerId.ToString());
        if (leagueMember == null)
        {
            var league = await dbContext.Leagues
                .FirstAsync(x => x.Id == leagueId);
            var (firstname, lastname) = GetFirstnameLastname(row.DisplayName ?? string.Empty);
            var member = new MemberEntity()
            {
                Firstname = firstname,
                Lastname = lastname,
                IRacingId = row.CustomerId.ToString(),
            };
            leagueMember = new LeagueMemberEntity()
            {
                Member = member,
                League = league,
            };
            dbContext.LeagueMembers.Add(leagueMember);
        }
        else
        {
            // update member name
            var (firstname, lastname) = GetFirstnameLastname(row.DisplayName ?? string.Empty);
            leagueMember.Member.Firstname = firstname;
            leagueMember.Member.Lastname = lastname;
        }
        return leagueMember;
    }

    private async Task<KeyValuePair<int, SessionResultEntity>> ReadSessionResultsAsync(long leagueId, SubSessionResult sessionData, SessionResults data,
        IRSimSessionDetailsEntity details, CancellationToken cancellationToken)
    {
        var sessionResult = new SessionResultEntity();
        sessionResult.LeagueId = leagueId;
        sessionResult.IRSimSessionDetails = details;
        sessionResult.SimSessionType = (SimSessionType)data.SimSessionType;
        var laps = data.Results.Max(x => x.LapsComplete);
        var resultRows = new List<ResultRowEntity>();
        foreach (var row in data.Results)
        {
            resultRows.Add(await ReadResultRowAsync(leagueId, sessionData, row, laps, cancellationToken));
        }
        sessionResult.ResultRows = resultRows;
        var sessionResultNr = data.SimSessionNumber;
        return new KeyValuePair<int, SessionResultEntity>(sessionResultNr, sessionResult);
    }

    private async Task<ResultRowEntity> ReadResultRowAsync(long leagueId, SubSessionResult sessionData, Result data,
        int laps, CancellationToken cancellationToken)
    {
        var row = new ResultRowEntity();
        var leagueMember = await GetOrCreateMemberAsync(leagueId, data, cancellationToken);
        row.LeagueId = leagueId;
        row.AvgLapTime = data.AverageLap ?? TimeSpan.Zero;
        //row.Car = FIXME is this data important??
        row.CarClass = sessionData.CarClasses.FirstOrDefault(x => x.CarClassId == data.CarClassId)?.ShortName ?? string.Empty;
        row.CarId = data.CarId;
        row.CarNumber = data.Livery.CarNumber ?? string.Empty;
        row.ClassId = data.CarClassId;
        row.ClubId = data.ClubId;
        row.ClubName = data.ClubName;
        row.CompletedLaps = data.LapsComplete;
        row.CompletedPct = laps != 0 ? data.LapsComplete / (double)laps : 0;
        row.ContactLaps = "";
        row.Division = data.Division;
        row.FastestLapTime = data.BestLapTime ?? TimeSpan.Zero;
        row.FastLapNr = data.BestLapNumber;
        row.FinishPosition = data.Position + 1;
        row.Incidents = data.Incidents;
        row.Interval = ParseInterval(data.Interval ?? TimeSpan.Zero, data.LapsComplete, laps);
        row.IRacingId = data.CustomerId.ToString();
        row.LeadLaps = data.LapsLead;
        row.License = sessionData.LicenseCategory;
        row.Member = leagueMember.Member;
        row.NewCpi = (int)data.NewCornersPerIncident;
        row.NewIRating = data.NewIRating;
        row.NewLicenseLevel = data.NewLicenseLevel;
        row.NewSafetyRating = data.NewSubLevel;
        row.NumContactLaps = -1;
        row.NumOfftrackLaps = -1;
        row.NumPitStops = -1;
        row.OfftrackLaps = "";
        row.OldCpi = (int)data.OldCornersPerIncident;
        row.OldIRating = data.OldIRating;
        row.OldLicenseLevel = data.OldLicenseLevel;
        row.OldSafetyRating = data.OldSubLevel;
        row.PittedLaps = "";
        row.PointsEligible = true;
        row.PositionChange = data.Position - data.StartingPosition;
        row.QualifyingTime = data.BestQualifyingLapTime ?? TimeSpan.Zero;
        row.QualifyingTimeAt = data.BestQualifyingLapAt?.UtcDateTime;
        row.SimSessionType = -1;
        row.StartPosition = data.StartingPosition + 1;
        row.Status = data.ReasonOutId;
        row.Team = leagueMember.Team;
        row.RacePoints = data.ChampPoints;
        row.SeasonStartIRating = SeasonStartIratings.TryGetValue(row.Member.Id, out int irating) ? irating : row.OldIRating;

        return row;
    }

    private IRSimSessionDetailsEntity ReadDetails(SubSessionResult data)
    {
        var details = new IRSimSessionDetailsEntity();

        details.IRSessionId = data.SessionId;
        details.IRSubsessionId = data.SubSessionId;
        details.IRTrackId = data.Track.TrackId;
        details.ConfigName = data.Track.ConfigName;
        details.Category = data.Track.Category;
        details.CornersPerLap = data.CornersPerLap;
        details.NumCautionLaps = data.NumberOfCautionLaps;
        details.NumCautions = data.NumberOfCautions;
        details.NumLeadChanges = data.NumberOfLeadChanges;
        details.WarmupRubber = data.TrackState.WarmupRubber;
        details.RaceRubber = data.TrackState.RaceRubber;
        details.DamageModel = data.DamageModel;
        details.EndTime = data.EndTime.UtcDateTime;
        details.EventAverageLap = ParseTime(data.EventAverageLap);
        details.EventLapsComplete = data.EventLapsComplete;
        details.EventStrengthOfField = data.EventStrengthOfField;
        details.Fog = data.Weather.Fog;
        details.IRRaceWeek = data.RaceWeekNumber;
        details.IRSeasonId = data.SeasonId;
        details.IRSeasonName = data.SeasonName;
        details.IRSeasonQuarter = data.SeasonQuarter;
        details.IRSeasonYear = data.SeasonYear;
        details.KmDistPerLap = 0;
        details.LeagueId = data.LeagueId ?? 0;
        details.LeaveMarbles = data.TrackState.LeaveMarbles;
        details.LicenseCategory = data.LicenseCategoryId;
        details.PracticeGripCompound = data.TrackState.PracticeGripCompound;
        details.PracticeRubber = data.TrackState.PracticeRubber;
        details.QualifyGripCompund = data.TrackState.QualifyGripCompound;
        details.QualifyRubber = data.TrackState.QualifyRubber;
        details.RaceGripCompound = data.TrackState.RaceGripCompound;
        details.RaceRubber = data.TrackState.RaceRubber;
        details.RelHumidity = data.Weather.RelHumidity;
        details.SessionName = data.SeasonName;
        details.SimStartUtcOffset = ParseTime(data.Weather.SimulatedStartUtcOffset);
        details.SimStartUtcTime = data.Weather.SimulatedStartUtcTime.UtcDateTime;
        details.Skies = data.Weather.Skies;
        details.StartTime = data.StartTime.UtcDateTime;
        details.TempUnits = data.Weather.TempUnits;
        details.TempValue = data.Weather.TempValue;
        details.TimeOfDay = data.Weather.TimeOfDay;
        details.TrackName = data.Track.TrackName;
        return details;
    }

    private async Task<EventEntity?> GetResultEventEntityAsync(long eventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Include(x => x.Schedule)
            .Include(x => x.Sessions)
            .Include(x => x.EventResult)
                .ThenInclude(x => x.SessionResults)
                    .ThenInclude(x => x.IRSimSessionDetails)
            .Include(x => x.SimSessionDetails)
            .Where(x => x.EventId == eventId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
