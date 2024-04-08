using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Excecution;
using Microsoft.AspNetCore.Connections.Features;
using System.Text.Json;
using System.Threading;
using System.Transactions;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record UploadResultRequest(long EventId, ParseSimSessionResult ResultData) : IRequest<bool>;

public sealed class UploadResultHandler : HandlerBase<UploadResultHandler, UploadResultRequest>,
    IRequestHandler<UploadResultRequest, bool>
{
    private readonly IResultCalculationQueue calculationQueue;
    private IDictionary<long, int> SeasonStartIratings;

    public UploadResultHandler(ILogger<UploadResultHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<UploadResultRequest>> validators,
        IResultCalculationQueue calculationQueue) :
        base(logger, dbContext, validators)
    {
        this.calculationQueue = calculationQueue;
        SeasonStartIratings = new Dictionary<long, int>();
    }

    public async Task<bool> Handle(UploadResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);

        // add to database
        var @event = await GetEventEntityAsync(request.EventId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        SeasonStartIratings = await GetMemberSeasonStartIratingAsync(@event.Schedule.SeasonId, cancellationToken);
        using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            if (@event.EventResult is not null)
            {
                dbContext.Remove(@event.EventResult);
            }
            @event.SimSessionDetails.Clear();
            await dbContext.SaveChangesAsync(cancellationToken);
            var result = await ReadResultsAsync(request.ResultData, @event, cancellationToken);
            @event.EventResult = result;
            await dbContext.SaveChangesAsync(cancellationToken);
            tx.Complete();
        }
        // Queue result calculation for this event
        await calculationQueue.QueueEventResultAsync(@event.EventId);

        return true;
    }

    private async Task<EventEntity?> GetEventEntityAsync(long eventId, CancellationToken cancellationToken)
    {
        // search for session first to check if result will be valid
        return await dbContext.Events
            .Where(x => x.EventId == eventId)
            .Include(x => x.Schedule)
            .Include(x => x.Sessions)
            .Include(x => x.EventResult)
                .ThenInclude(x => x.SessionResults)
                    .ThenInclude(x => x.IRSimSessionDetails)
            .Include(x => x.SimSessionDetails)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<ParseSimSessionResult> ParseDataStream(Stream dataStream, CancellationToken cancellationToken)
    {
        return await JsonSerializer.DeserializeAsync<ParseSimSessionResult>(dataStream, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to parse results from json");
    }

    private async Task<EventResultEntity> ReadResultsAsync(ParseSimSessionResult data, EventEntity @event,
        CancellationToken cancellationToken)
    {
        var map = CreateSessionMapping(data.session_results, @event);
        // create entities
        var result = @event.EventResult ?? new EventResultEntity();
        var details = ReadDetails(data);
        details.Event = @event;
        IDictionary<int, SessionResultEntity> sessionResults = new Dictionary<int, SessionResultEntity>();
        foreach (var sessionResultData in data.session_results)
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

    private static ICollection<(int resultNr, int sessionNr)> CreateSessionMapping(IEnumerable<ParseSessionResult> sessionResults, EventEntity @event)
    {
        var map = new List<(int resultNr, int sessionNr)>();

        var practiceSessionTypes = new[] { SimSessionType.OpenPractice }.Cast<int>();
        var practiceSession = @event.Sessions
            .FirstOrDefault(x => x.SessionType == SessionType.Practice);
        var practiceResult = sessionResults
            .FirstOrDefault(x => practiceSessionTypes.Contains(x.simsession_type) && x.simsession_name == "PRACTICE");
        if (practiceSession is not null && practiceResult is not null)
        {
            map.Add((practiceResult.simsession_number, practiceSession.SessionNr));
        }

        var qualySessionTypes = new[] { SimSessionType.LoneQualifying, SimSessionType.OpenQualifying }.Cast<int>();
        var qualySession = @event.Sessions
            .FirstOrDefault(x => x.SessionType == SessionType.Qualifying);
        var qualyResult = sessionResults
            .FirstOrDefault(x => qualySessionTypes.Contains(x.simsession_type));
        if (qualySession is not null && qualyResult is not null)
        {
            map.Add((qualyResult.simsession_number, qualySession.SessionNr));
        }

        var raceSessionTypes = new[] { SimSessionType.Race }.Cast<int>();
        var raceSessions = @event.Sessions
            .OrderBy(x => x.SessionNr)
            .Where(x => x.SessionType == SessionType.Race);
        var raceResults = sessionResults
            .Where(x => raceSessionTypes.Contains(x.simsession_type)).Reverse();
        foreach ((var session, var result) in raceSessions.Zip(raceResults))
        {
            if (session is not null && result is not null)
            {
                map.Add((result.simsession_number, session.SessionNr));
            }
        }

        return map;
    }

    private async Task<LeagueMemberEntity> GetOrCreateMemberAsync(long leagueId, ParseSessionResultRow row, CancellationToken cancellationToken)
    {
        var custId = row.cust_id.ToString();
        var displayName = row.display_name ?? string.Empty;
        var leagueMember = await dbContext.LeagueMembers
            .Include(x => x.Team)
            .Include(x => x.Member)
            .Where(x => x.LeagueId == leagueId)
            .Where(x => x.Member.IRacingId == custId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? dbContext.LeagueMembers.Local
            .SingleOrDefault(x => x.Member.IRacingId == custId);
        if (leagueMember == null)
        {
            var league = await dbContext.Leagues
                .FirstAsync(x => x.Id == leagueId);
            var (firstname, lastname) = GetFirstnameLastname(displayName);
            var member = new MemberEntity()
            {
                Firstname = firstname,
                Lastname = lastname,
                IRacingId = custId.ToString(),
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
            var (firstname, lastname) = GetFirstnameLastname(displayName);
            leagueMember.Member.Firstname = firstname;
            leagueMember.Member.Lastname = lastname;
        }
        return leagueMember;
    }

    private async Task<TeamEntity?> GetTeamAsync(long? iracingTeamId, CancellationToken cancellationToken)
    {
        if (iracingTeamId is null)
        {
            return null;
        }

        iracingTeamId = Math.Abs(iracingTeamId.Value); // get positive value because team is negative in result
        return await dbContext.Teams
            .SingleOrDefaultAsync(x => x.IRacingTeamId == iracingTeamId, cancellationToken)
            ?? dbContext.Teams.Local
            .SingleOrDefault(x => x.IRacingTeamId == iracingTeamId);
    }

    private async Task UpdateOrCreateTeamAsync(long leagueId, ParseSessionResultRow row, CancellationToken cancellationToken)
    {
        if (row.team_id is null)
        {
            return;
        }

        var iracingTeamId = Math.Abs(row.team_id.Value); // get positive value because team is negative in result
        var teamName = row.display_name ?? string.Empty;
        var team = await GetTeamAsync(iracingTeamId, cancellationToken);
        if (team is null)
        {
            var league = await dbContext.Leagues
                .FirstAsync(x => x.Id == leagueId, cancellationToken);
            team = new TeamEntity()
            {
                League = league,
                LeagueId = league.Id,
                IRacingTeamId = iracingTeamId,
                Name = teamName,
            };
            dbContext.Teams.Add(team);
        }

        foreach(var driverRow in row.driver_results)
        {
            var member = await GetOrCreateMemberAsync(leagueId, driverRow, cancellationToken);
            member.Team = team;
        }
    }

    private async Task<KeyValuePair<int, SessionResultEntity>> ReadSessionResultsAsync(long leagueId, ParseSimSessionResult sessionData, ParseSessionResult data,
        IRSimSessionDetailsEntity details, CancellationToken cancellationToken)
    {
        var sessionResult = new SessionResultEntity
        {
            LeagueId = leagueId,
            IRSimSessionDetails = details,
            SimSessionType = (SimSessionType)data.simsession_type
        };
        var laps = data.results.Max(x => x.laps_complete);
        var resultRows = new List<ResultRowEntity>();
        bool isTeamResult = data.results.Any(x => x.driver_results.Any());
        var dataRows = data.results.AsEnumerable();
        if (isTeamResult)
        {
            foreach(var teamRow in dataRows)
            {
                await UpdateOrCreateTeamAsync(leagueId, teamRow, cancellationToken);
            }
            dataRows = data.results.SelectMany(x => x.driver_results);
        }
        foreach (var row in dataRows)
        {
            resultRows.Add(await ReadResultRowAsync(leagueId, sessionData, row, laps, cancellationToken));
        }
        sessionResult.ResultRows = resultRows;
        var sessionResultNr = data.simsession_number;
        return new KeyValuePair<int, SessionResultEntity>(sessionResultNr, sessionResult);
    }

    private IRSimSessionDetailsEntity ReadDetails(ParseSimSessionResult data)
    {
        var details = new IRSimSessionDetailsEntity
        {
            IRSessionId = data.session_id,
            IRSubsessionId = data.subsession_id,
            IRTrackId = data.track.track_id,
            ConfigName = data.track.config_name,
            Category = data.track.category,
            CornersPerLap = data.corners_per_lap,
            NumCautionLaps = data.num_caution_laps,
            NumCautions = data.num_cautions,
            NumLeadChanges = data.num_lead_changes,
            WarmupRubber = data.track_state.warmup_rubber,
            RaceRubber = data.track_state.race_rubber,
            DamageModel = data.damage_model,
            EndTime = data.end_time,
            EventAverageLap = ParseTime(data.event_average_lap),
            EventLapsComplete = data.event_laps_complete,
            EventStrengthOfField = data.event_strength_of_field,
            Fog = data.weather.fog,
            IRRaceWeek = data.race_week_num,
            IRSeasonId = data.season_id,
            IRSeasonName = data.season_name,
            IRSeasonQuarter = data.season_quarter,
            IRSeasonYear = data.season_year,
            KmDistPerLap = 0,
            LeagueId = data.league_id,
            LeaveMarbles = data.track_state.leave_marbles,
            LicenseCategory = data.license_category_id,
            PracticeGripCompound = data.track_state.practice_grip_compound,
            PracticeRubber = data.track_state.practice_rubber,
            QualifyGripCompund = data.track_state.qualify_grip_compound,
            QualifyRubber = data.track_state.qualify_rubber,
            RaceGripCompound = data.track_state.race_grip_compound,
            RelHumidity = data.weather.rel_humidity,
            SessionName = data.session_name,
            SimStartUtcOffset = ParseTime(data.weather.simulated_start_utc_offset),
            SimStartUtcTime = data.weather.simulated_start_utc_time,
            Skies = data.weather.skies,
            StartTime = data.start_time,
            TempUnits = data.weather.temp_units,
            TempValue = data.weather.temp_value,
            TimeOfDay = data.weather.time_of_day,
            TrackName = data.track.track_name
        };
        return details;
    }

    private async Task<ResultRowEntity> ReadResultRowAsync(long leagueId, ParseSimSessionResult sessionData, ParseSessionResultRow data,
        int laps, CancellationToken cancellationToken)
    {
        var leagueMember = await GetOrCreateMemberAsync(leagueId, data, cancellationToken);
        var team = await GetTeamAsync(data.team_id, cancellationToken)
            ?? leagueMember.Team;
        var row = new ResultRowEntity
        {
            LeagueId = leagueId,
            AvgLapTime = ParseTime(data.average_lap),
            Car = data.car_name,
            CarClass = sessionData.car_classes.FirstOrDefault(x => x.car_class_id == data.car_class_id)?.short_name ?? string.Empty,
            CarId = data.car_id,
            CarNumber = data.livery.car_number ?? string.Empty,
            ClassId = data.car_class_id,
            ClubId = data.club_id,
            ClubName = data.club_name,
            CompletedLaps = data.laps_complete,
            CompletedPct = laps != 0 ? data.laps_complete / (double)laps : 0,
            ContactLaps = "",
            Division = data.division,
            FastestLapTime = ParseTime(data.best_lap_time),
            FastLapNr = data.best_lap_num,
            FinishPosition = data.position + 1,
            Incidents = data.incidents,
            Interval = ParseInterval(data.class_interval, data.laps_complete, laps),
            IRacingId = data.cust_id.ToString(),
            LeadLaps = data.laps_lead,
            License = sessionData.license_category,
            Member = leagueMember.Member,
            NewCpi = (int)data.new_cpi,
            NewIRating = data.newi_rating,
            NewLicenseLevel = data.new_license_level,
            NewSafetyRating = data.new_sub_level,
            NumContactLaps = -1,
            NumOfftrackLaps = -1,
            NumPitStops = -1,
            OfftrackLaps = "",
            OldCpi = (int)data.old_cpi,
            OldIRating = data.oldi_rating,
            OldLicenseLevel = data.old_license_level,
            OldSafetyRating = data.old_sub_level,
            PittedLaps = "",
            PointsEligible = true,
            PositionChange = data.position - data.starting_position,
            QualifyingTime = ParseTime(data.best_qual_lap_time),
            QualifyingTimeAt = data.best_qual_lap_at,
            SimSessionType = -1,
            StartPosition = data.starting_position + 1,
            Status = data.reason_out_id,
            Team = team,
            RacePoints = data.champ_points
        };
        row.SeasonStartIRating = SeasonStartIratings.TryGetValue(row.Member.Id, out int irating) ? irating : row.OldIRating;

        return row;
    }
}
