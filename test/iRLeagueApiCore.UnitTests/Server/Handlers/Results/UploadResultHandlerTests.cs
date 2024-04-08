using AutoFixture.Dsl;
using FluentValidation;
using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Server.Handlers.Results;
using iRLeagueApiCore.Services.ResultService.Excecution;
using iRLeagueApiCore.UnitTests.Extensions;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Results;

public sealed class UploadResultHandlerTests : DataAccessTestsBase
{
    private readonly IResultCalculationQueue calculationQueue;
    private readonly ILogger<UploadResultHandler> logger;

    public UploadResultHandlerTests() : base()
    {
        calculationQueue = Mock.Of<IResultCalculationQueue>();
        logger = Mock.Of<ILogger<UploadResultHandler>>();
    }

    [Fact]
    public async Task CreateFakeResult_ShouldReturnDefaultResult()
    {
        var result = await CreateFakeResult();

        result.session_results.Should().HaveCount(3);
        result.session_results[0].simsession_name.Should().Be("RACE");
        result.session_results[0].simsession_number.Should().Be(0);
        result.session_results[0].simsession_type.Should().Be((int)SimSessionType.Race);
        result.session_results[0].simsession_type_name.Should().Be("Race");
        result.session_results[1].simsession_name.Should().Be("QUALIFY");
        result.session_results[1].simsession_number.Should().Be(-1);
        result.session_results[1].simsession_type.Should().Be((int)SimSessionType.LoneQualifying);
        result.session_results[1].simsession_type_name.Should().Be("Lone Qualifying");
        result.session_results[2].simsession_name.Should().Be("PRACTICE");
        result.session_results[2].simsession_number.Should().Be(-2);
        result.session_results[2].simsession_type.Should().Be((int)SimSessionType.OpenPractice);
        result.session_results[2].simsession_type_name.Should().Be("Open Practice");
    }

    [Fact]
    public async Task Handle_ShouldCreateMembers_WhenMemberDoesNotExist()
    {
        var rowCount = 10;
        var names = Enumerable.Range(0, rowCount).Select(x =>
            new { Firstname = fixture.Create<string>(), Lastname = fixture.Create<string>() })
            .ToArray();
        var newMemberRows = names.Select((x, i) => fixture.Build<ParseSessionResultRow>()
            .With(x => x.cust_id, dbContext.Members.Select(x => Convert.ToInt64(x.IRacingId)).Max() + 1 + i)
            .With(x => x.display_name, $"{x.Firstname} {x.Lastname}")
            .Without(x => x.driver_results)
            .Create())
            .ToArray();
        var result = await CreateFakeResult(practice: false, qualy: false, raceCount: 1);
        result.session_results.First().results = result.session_results.First().results.Concat(newMemberRows).ToArray();
        var sut = CreateSut();
        var request = CreateRequest(TestEventId, result);

        await sut.Handle(request, default);

        foreach ((var newMemberRow, var name) in newMemberRows.Zip(names))
        {
            var newMember = await dbContext.Members
            .FirstOrDefaultAsync(x => x.IRacingId == newMemberRow.cust_id.ToString());
            newMember.Should().NotBeNull();
            newMember!.Firstname.Should().Be(name.Firstname);
            newMember.Lastname.Should().Be(name.Lastname);
        }
    }

    [Fact]
    public async Task Handle_ShouldCreateTeam_WhenTeamDoesNotExist()
    {
        var rowCount = 2;
        var names = Enumerable.Range(0, rowCount).Select(x =>
            new { Firstname = fixture.Create<string>(), Lastname = fixture.Create<string>() })
            .ToArray();
        var members = await dbContext.LeagueMembers
            .Where(x => x.Team == null)
            .Take(rowCount)
            .ToListAsync();
        var newTeam = fixture.Build<TeamEntity>()
            .With(x => x.Members, members)
            .Without(x => x.League)
            .Without(x => x.InvolvedReviews)
            .Create();
        var newTeamRow = CreateTeamResultRow(1, (newTeam.IRacingTeamId!.Value, newTeam.Name, members));
        var result = await CreateFakeResult(practice: false, qualy: false, teamResult: true, raceCount: 1);
        result.session_results.First().results = result.session_results.First().results.Concat(new[] { newTeamRow }).ToArray();
        var sut = CreateSut();
        var request = CreateRequest(TestEventId, result);

        await sut.Handle(request, default);

        var testNewTeam = await dbContext.Teams
            .FirstOrDefaultAsync(x => x.IRacingTeamId == newTeam.IRacingTeamId);
        testNewTeam.Should().NotBeNull();
        testNewTeam!.Name.Should().Be(newTeam.Name);
        testNewTeam.Members.Should().Contain(members);
    }

    [Fact]
    public async Task Handle_ShouldAddMemberToTeam_WhenDriverIsInNoTeam()
    {
        var team = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync();
        var addMember = await dbContext.LeagueMembers
            .Where(x => x.Team == null)
            .FirstAsync();
        var rowCount = team.Members.Count();
        var members = team.Members
            .ToList();
        Debug.Assert(members.Contains(addMember) == false);
        members.Add(addMember);
        var updateTeamRow = CreateTeamResultRow(1, (team.IRacingTeamId!.Value, team.Name, members));
        var result = await CreateFakeResult(practice: false, qualy: false, teamResult: true, raceCount: 1);
        result.session_results.First().results = result.session_results.First().results
            .Concat(new[] { updateTeamRow })
            .ToArray();
        var sut = CreateSut();
        var request = CreateRequest(TestEventId, result);

        await sut.Handle(request, default);

        var testUpdateTeam = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync(x => x.TeamId == team.TeamId);
        testUpdateTeam.Members.Should().Contain(addMember);
    }

    [Fact]
    public async Task Handle_ShouldMoveMemberToTeam_WhenDriverIsInAnotherTeam()
    {
        var team = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync();
        var oldTeam = await dbContext.Teams
            .Include(x => x.Members)
            .Skip(1)
            .FirstAsync();
        var moveMember = oldTeam.Members.First();
        var rowCount = team.Members.Count();
        var members = team.Members
            .ToList();
        Debug.Assert(members.Contains(moveMember) == false);
        members.Add(moveMember);
        var updateTeamRow = CreateTeamResultRow(1, (team.IRacingTeamId!.Value, team.Name, members));
        var result = await CreateFakeResult(practice: false, qualy: false, teamResult: true, raceCount: 1);
        result.session_results.First().results = result.session_results.First().results
            .Concat(new[] { updateTeamRow })
            .ToArray();
        var sut = CreateSut();
        var request = CreateRequest(TestEventId, result);

        await sut.Handle(request, default);

        var testUpdateTeam = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync(x => x.TeamId == team.TeamId);
        testUpdateTeam.Members.Should().Contain(moveMember);
        var testUpdateOldTeam = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync(x => x.TeamId == oldTeam.TeamId);
        testUpdateOldTeam.Members.Should().NotContain(moveMember);
    }

    [Fact]
    public async Task Handle_ShouldRemoveMemberFromTeam_WhenDriverHasNoTeamInResult()
    {
        var team = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync();
        var removeMember = team.Members.First();
        var members = team.Members.ToList();
        members.Remove(removeMember);
        var updateTeamRow = CreateTeamResultRow(1, (team.IRacingTeamId!.Value, team.Name, members));
        var result = await CreateFakeResult(practice: false, qualy: false, teamResult: true, raceCount: 1);
        result.session_results.First().results = result.session_results.First().results
            .Concat(new[] { updateTeamRow })
            .ToArray();
        var sut = CreateSut();
        var request = CreateRequest(TestEventId, result);

        await sut.Handle(request, default);

        var testUpdateTeam = await dbContext.Teams
            .Include(x => x.Members)
            .FirstAsync(x => x.TeamId == team.TeamId);
        testUpdateTeam.Members.Should().NotContain(removeMember);
    }

    [Fact]
    public async Task Handle_ShouldAssingPracticeResults()
    {
        var @event = EventBuilder().Create();
        var session = SessionBuilder()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.Event, @event)
            .With(x => x.SessionType, SessionType.Practice)
            .Create();
        @event.Sessions.Add(session);
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult();
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var testSession = await dbContext.Sessions
            .Include(x => x.SessionResult)
            .FirstAsync(x => x.SessionId == session.SessionId);
        var sessionResult = testSession.SessionResult;
        sessionResult.Should().NotBeNull();
        sessionResult.Session.SessionId.Should().Be(session.SessionId);
        sessionResult.SimSessionType.Should().Be(SimSessionType.OpenPractice);
    }

    [Fact]
    public async Task Handle_ShouldAssingQualifyingResults()
    {
        var @event = EventBuilder().Create();
        var session = SessionBuilder()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.Event, @event)
            .With(x => x.SessionType, SessionType.Qualifying)
            .Create();
        @event.Sessions.Add(session);
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult();
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var testSession = await dbContext.Sessions
            .Include(x => x.SessionResult)
            .FirstAsync(x => x.SessionId == session.SessionId);
        var sessionResult = testSession.SessionResult;
        sessionResult.Should().NotBeNull();
        sessionResult.SimSessionType.Should().Be(SimSessionType.LoneQualifying);
    }

    [Fact]
    public async Task Handle_ShouldAssignSingleRace()
    {
        var @event = EventBuilder().Create();
        var session = SessionBuilder()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.Event, @event)
            .With(x => x.SessionType, SessionType.Race)
            .Create();
        @event.Sessions.Add(session);
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult();
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var testSession = await dbContext.Sessions
            .Include(x => x.SessionResult)
            .FirstAsync(x => x.SessionId == session.SessionId);
        var sessionResult = testSession.SessionResult;
        sessionResult.Should().NotBeNull();
        sessionResult.SimSessionType.Should().Be(SimSessionType.Race);
    }

    [Fact]
    public async Task Handle_ShouldAssignHeatRaces()
    {
        var sessionCount = 3;
        var @event = EventBuilder().Create();
        @event.Sessions = SessionBuilder()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.Event, @event)
            .With(x => x.SessionType, SessionType.Race)
            .With(x => x.Name, "Heat")
            .CreateMany(sessionCount)
            .ToList();
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult(raceCount: sessionCount);
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var raceResults = result.session_results
            .Where(x => x.simsession_type == (int)SimSessionType.Race)
            .Reverse();
        var testSessions = await dbContext.Sessions
            .Include(x => x.SessionResult)
                .ThenInclude(x => x.ResultRows)
            .Where(x => x.EventId == @event.EventId)
            .OrderBy(x => x.SessionNr)
            .ToListAsync();
        testSessions[0].SessionResult.Should().NotBeNull();
        foreach ((var testSession, var sessionResult) in testSessions.OrderBy(x => x.SessionNr).Zip(raceResults))
        {
            foreach ((var testRow, var resultRow) in testSession.SessionResult.ResultRows.OrderBy(x => x.FinishPosition).Zip(sessionResult.results))
            {
                testRow.FinishPosition.Should().Be(resultRow.position+1);
                testRow.Member.IRacingId.Should().Be(resultRow.cust_id.ToString());
            }
        }
    }

    [Fact]
    public async Task Handle_ShouldFillResultRowData()
    {
        var @event = EventBuilder().Create();
        @event.Sessions.Add(SessionBuilder()
            .With(x => x.SessionType, SessionType.Race)
            .Create());
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult(practice: false, qualy: false, raceCount: 1);
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var sessionResult = await dbContext.SessionResults
            .Include(x => x.ResultRows)
                .ThenInclude(x => x.Member)
            .FirstAsync(x => x.EventId == @event.EventId);
        var laps = result.session_results.First().results.Select(y => y.laps_complete).Max();
        foreach ((var testRow, var resultRow) in sessionResult.ResultRows.Zip(result.session_results.First().results))
        {
            testRow.AvgLapTime.Should().Be(TimeSpan.FromSeconds(resultRow.average_lap / 10000D));
            testRow.Car.Should().Be(resultRow.car_name);
            testRow.CarId.Should().Be(resultRow.car_id);
            testRow.CarNumber.ToString().Should().Be(resultRow.livery.car_number);
            testRow.ClassId.Should().Be(resultRow.car_class_id);
            testRow.ClubId.Should().Be(resultRow.club_id);
            testRow.CompletedLaps.Should().Be(resultRow.laps_complete);
            testRow.CompletedPct.Should().BeApproximately(resultRow.laps_complete / (double)laps, 0.001);
            testRow.Division.Should().Be(resultRow.division);
            testRow.FastestLapTime.Should().Be(TimeSpan.FromSeconds(resultRow.best_lap_time / 10000D));
            testRow.FastLapNr.Should().Be(resultRow.best_lap_num);
            testRow.FinishPosition.Should().Be(resultRow.position+1);
            testRow.Incidents.Should().Be(resultRow.incidents);
            testRow.Interval.Should().Be(TimeSpan.FromSeconds(resultRow.class_interval / 10000D));
            testRow.IRacingId.Should().Be(resultRow.cust_id.ToString());
            testRow.LeadLaps.Should().Be(resultRow.laps_lead);
            testRow.Member.IRacingId.Should().Be(resultRow.cust_id.ToString());
            testRow.NewCpi.Should().Be((int)resultRow.new_cpi);
            testRow.NewIRating.Should().Be(resultRow.newi_rating);
            testRow.NewLicenseLevel.Should().Be(resultRow.new_license_level);
            testRow.NewSafetyRating.Should().Be(resultRow.new_sub_level);
            testRow.OldCpi.Should().Be((int)resultRow.old_cpi);
            testRow.OldIRating.Should().Be(resultRow.oldi_rating);
            testRow.OldLicenseLevel.Should().Be(resultRow.old_license_level);
            testRow.OldSafetyRating.Should().Be(resultRow.old_sub_level);
            testRow.PointsEligible.Should().Be(true);
            testRow.PositionChange.Should().Be(resultRow.position - resultRow.starting_position);
            testRow.QualifyingTime.Should().Be(TimeSpan.FromSeconds(resultRow.best_qual_lap_time / 10000D));
            testRow.QualifyingTimeAt.Should().Be(resultRow.best_qual_lap_at);
            testRow.StartPosition.Should().Be(resultRow.starting_position + 1);
            testRow.Status.Should().Be(resultRow.reason_out_id);
        }
    }

    [Fact]
    public async Task Handle_ShouldGetIntervalLaps_WhenIntervalIsNegative()
    {
        var @event = EventBuilder().Create();
        @event.Sessions.Add(SessionBuilder()
            .With(x => x.SessionType, SessionType.Race)
            .Create());
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult(practice: false, qualy: false, raceCount: 1);
        var maxLaps = result.session_results[0].results.Max(x => x.laps_complete);
        result.session_results[0].results.First().laps_complete = maxLaps;
        var lapsDown = 3;
        var setupRow = result.session_results[0].results.Last();
        setupRow.class_interval = -1;
        setupRow.laps_complete = maxLaps - lapsDown;
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var session = await dbContext.SessionResults
            .Include(x => x.ResultRows)
                .ThenInclude(x => x.Member)
            .FirstAsync(x => x.EventId == @event.EventId);
        var testRow = session.ResultRows.Last();
        testRow.Interval.Should().Be(TimeSpan.FromDays(lapsDown));
    }

    [Fact]
    public async Task Handle_ShouldFillResultRowData_FromTeamResults()
    {
        var @event = EventBuilder().Create();
        @event.Sessions.Add(SessionBuilder()
            .With(x => x.SessionType, SessionType.Race)
            .Create());
        dbContext.Events.Add(@event);
        await dbContext.SaveChangesAsync();
        var result = await CreateFakeResult(practice: false, qualy: false, teamResult: true, raceCount: 1);
        var request = CreateRequest(@event.EventId, result);
        var sut = CreateSut();

        await sut.Handle(request, default);

        var sessionResult = await dbContext.SessionResults
            .Include(x => x.ResultRows)
                .ThenInclude(x => x.Member)
            .FirstAsync(x => x.EventId == @event.EventId);
        var laps = result.session_results.First().results.Select(y => y.laps_complete).Max();
        foreach ((var testRow, var resultRow) in sessionResult.ResultRows.Zip(result.session_results.First().results.SelectMany(x => x.driver_results)))
        {
            testRow.AvgLapTime.Should().Be(TimeSpan.FromSeconds(resultRow.average_lap / 10000D));
            testRow.Car.Should().Be(resultRow.car_name);
            testRow.CarId.Should().Be(resultRow.car_id);
            testRow.CarNumber.ToString().Should().Be(resultRow.livery.car_number);
            testRow.ClassId.Should().Be(resultRow.car_class_id);
            testRow.ClubId.Should().Be(resultRow.club_id);
            testRow.CompletedLaps.Should().Be(resultRow.laps_complete);
            testRow.CompletedPct.Should().BeApproximately(resultRow.laps_complete / (double)laps, 0.001);
            testRow.Division.Should().Be(resultRow.division);
            testRow.FastestLapTime.Should().Be(TimeSpan.FromSeconds(resultRow.best_lap_time / 10000D));
            testRow.FastLapNr.Should().Be(resultRow.best_lap_num);
            testRow.FinishPosition.Should().Be(resultRow.position + 1);
            testRow.Incidents.Should().Be(resultRow.incidents);
            testRow.Interval.Should().Be(TimeSpan.FromSeconds(resultRow.class_interval / 10000D));
            testRow.IRacingId.Should().Be(resultRow.cust_id.ToString());
            testRow.LeadLaps.Should().Be(resultRow.laps_lead);
            testRow.Member.IRacingId.Should().Be(resultRow.cust_id.ToString());
            testRow.NewCpi.Should().Be((int)resultRow.new_cpi);
            testRow.NewIRating.Should().Be(resultRow.newi_rating);
            testRow.NewLicenseLevel.Should().Be(resultRow.new_license_level);
            testRow.NewSafetyRating.Should().Be(resultRow.new_sub_level);
            testRow.OldCpi.Should().Be((int)resultRow.old_cpi);
            testRow.OldIRating.Should().Be(resultRow.oldi_rating);
            testRow.OldLicenseLevel.Should().Be(resultRow.old_license_level);
            testRow.OldSafetyRating.Should().Be(resultRow.old_sub_level);
            testRow.PointsEligible.Should().Be(true);
            testRow.PositionChange.Should().Be(resultRow.position - resultRow.starting_position);
            testRow.QualifyingTime.Should().Be(TimeSpan.FromSeconds(resultRow.best_qual_lap_time / 10000D));
            testRow.QualifyingTimeAt.Should().Be(resultRow.best_qual_lap_at);
            testRow.StartPosition.Should().Be(resultRow.starting_position + 1);
            testRow.Status.Should().Be(resultRow.reason_out_id);
        }
    }

    private IPostprocessComposer<EventEntity> EventBuilder()
    {
        return fixture.Build<EventEntity>()
            .With(x => x.Schedule, dbContext.Schedules.First())
            .With(x => x.EventId, () => dbContext.Events.Select(x => x.EventId).Max() + 1)
            .Without(x => x.LeagueId)
            .Without(x => x.EventResult)
            .Without(x => x.ResultConfigs)
            .Without(x => x.ScoredEventResults)
            .Without(x => x.Sessions)
            .Without(x => x.Track)
            .Without(x => x.SimSessionDetails);
    }

    private IPostprocessComposer<SessionEntity> SessionBuilder()
    {
        return fixture.Build<SessionEntity>()
            .With(x => x.SessionId, () => dbContext.Sessions.Select(x => x.SessionId).Max() + fixture.Create<int>())
            .Without(x => x.Event)
            .Without(x => x.IncidentReviews)
            .Without(x => x.SessionResult);
    }

    private UploadResultHandler CreateSut()
    {
        return new UploadResultHandler(logger, dbContext, Array.Empty<IValidator<UploadResultRequest>>(), calculationQueue);
    }

    private UploadResultRequest CreateRequest(long eventId, ParseSimSessionResult data)
    {
        return new UploadResultRequest(eventId, data);
    }

    private async Task<ParseSimSessionResult> CreateFakeResult(
        bool practice = true,
        bool qualy = true,
        bool teamResult = false,
        int raceCount = 1)
    {
        var memberCount = 10;
        var members = await dbContext.LeagueMembers
            .Include(x => x.Member)
            .Take(memberCount)
            .ToListAsync();
        var result = fixture.Build<ParseSimSessionResult>()
            .Without(x => x.session_results)
            .Create();
        var sessionResults = new List<ParseSessionResult>();
        var sessionNr = (practice ? -1 : 0) + (qualy ? -1 : 0) - raceCount * 2 + 1;
        if (practice)
        {
            sessionResults.Add(SessionResultBuilder(members, teamResult)
                .With(x => x.simsession_number, ++sessionNr)
                .With(x => x.simsession_type, (int)SimSessionType.OpenPractice)
                .With(x => x.simsession_type_name, "Open Practice")
                .With(x => x.simsession_name, "PRACTICE")
                .Create());
        }
        if (qualy)
        {
            sessionResults.Add(SessionResultBuilder(members, teamResult)
                .With(x => x.simsession_number, ++sessionNr)
                .With(x => x.simsession_type, (int)SimSessionType.LoneQualifying)
                .With(x => x.simsession_type_name, "Lone Qualifying")
                .With(x => x.simsession_name, "QUALIFY")
                .Create());

        }
        var heatNr = 1;
        for (int i = 0; i < raceCount; i++)
        {
            sessionResults.Add(SessionResultBuilder(members, teamResult)
                .With(x => x.simsession_number, ++sessionNr)
                .With(x => x.simsession_type, (int)SimSessionType.Race)
                .With(x => x.simsession_type_name, "Race")
                .With(x => x.simsession_name, raceCount == 1 ? "RACE" : sessionNr == 0 ? "FEATURE" : $"HEAT {heatNr++}")
                .Create());
            if (sessionNr != 0)
            {
                sessionResults.Add(SessionResultBuilder(members, teamResult)
                    .With(x => x.simsession_number, ++sessionNr)
                    .With(x => x.simsession_type, (int)SimSessionType.OpenPractice)
                    .With(x => x.simsession_type_name, "Open Practice")
                    .With(x => x.simsession_name, "WARMUP")
                    .Create());
            }
        }
        sessionResults.Reverse();
        result.session_results = sessionResults.ToArray();
        return result;
    }

    private IPostprocessComposer<ParseSessionResult> SessionResultBuilder(IEnumerable<LeagueMemberEntity> members, bool teamResult = false)
    {
        if (teamResult)
        {
            var teams = members.Chunk(3)
                .Select(x => (fixture.Create<long>(), fixture.Create<string>(), x.AsEnumerable()));
            return SessionTeamResultBuilder(teams);
        }

        return fixture.Build<ParseSessionResult>()
            .With(x => x.results, members.Shuffle()
                .Select((member, i) => CreateResultRow(i + 1, member))
                .ToArray());
    }

    private IPostprocessComposer<ParseSessionResult> SessionTeamResultBuilder(IEnumerable<(long teamId, string teamName, IEnumerable<LeagueMemberEntity> members)> teams)
    {
        return fixture.Build<ParseSessionResult>()
            .With(x => x.results, teams.Shuffle()
                .Select((team, i) => CreateTeamResultRow(i + 1, team))
                .ToArray());
    }

    private ParseSessionResultRow CreateResultRow(int pos, LeagueMemberEntity leagueMember, long? teamId = null)
    {
        return fixture.Build<ParseSessionResultRow>()
            .With(x => x.team_id, teamId)
            .With(x => x.position, pos)
            .With(x => x.cust_id, Convert.ToInt64(leagueMember.Member.IRacingId))
            .With(x => x.display_name, GetMemberFullName(leagueMember.Member))
            .With(x => x.livery, fixture.Build<ParseLivery>()
                .With(x => x.car_number, fixture.Create<int>().ToString())
                .Create())
            .Without(x => x.driver_results)
            .Create();
    }

    private ParseSessionResultRow CreateTeamResultRow(int pos, (long teamId, string name, IEnumerable<LeagueMemberEntity> members) team)
    {
        return fixture.Build<ParseSessionResultRow>()
            .With(x => x.position, pos)
            .With(x => x.team_id, team.teamId)
            .With(x => x.display_name, team.name)
            .With(x => x.livery, fixture.Build<ParseLivery>()
                .With(x => x.car_number, fixture.Create<int>().ToString())
                .Create())
            .With(x => x.driver_results, team.members
                .Select(x => CreateResultRow(pos, x, teamId: team.teamId))
                .ToArray())
            .Create();
    }

    private string GetMemberFullName(MemberEntity member)
    {
        return $"{member.Firstname} {member.Firstname}";
    }
}
