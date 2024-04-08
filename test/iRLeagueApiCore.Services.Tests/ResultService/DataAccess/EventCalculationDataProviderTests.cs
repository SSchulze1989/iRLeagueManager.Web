using DbIntegrationTests;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueApiCore.Services.ResultService.DataAccess;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.Tests.ResultService.DataAccess;

public sealed class EventCalculationDataProviderTests : DataAccessTestsBase
{
    [Fact]
    public async Task GetData_ShouldReturnNull_WhenNoResultDataExists()
    {
        var schedule = await dbContext.Schedules.FirstAsync();
        var @event = accessMockHelper.EventBuilder(schedule).Create();
        var config = GetConfiguration(@event);
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().BeNull();
    }

    [Fact]
    public async Task GetData_ShouldReturnData_WhenResultDataExists()
    {
        var @event = await GetFirstEventEntity();
        var rawResult = @event.EventResult;
        var config = GetConfiguration(@event);
        var sut = CreateSut();

        var test = (await sut.GetData(config))!;

        test.Should().NotBeNull();
        test.LeagueId = @event.LeagueId;
        test.EventId = @event.EventId;
        test.SessionResults.Should().HaveSameCount(@event.Sessions);
        test.SessionResults.Should().HaveSameCount(rawResult.SessionResults);
        foreach ((var sessionResultData, var session, var sessionResult) in test.SessionResults.Zip(@event.Sessions, rawResult.SessionResults))
        {
            sessionResultData.LeagueId.Should().Be(session.LeagueId);
            sessionResultData.SessionId.Should().Be(session.SessionId);
            sessionResultData.SessionId.Should().Be(sessionResult.SessionId);
            sessionResultData.ResultRows.Should().HaveSameCount(sessionResult.ResultRows);
            sessionResultData.AcceptedReviewVotes.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetData_ShouldProvideAcceptedReviewVotes_WhenAcceptedVoteExists()
    {
        var @event = await GetFirstEventEntity();
        var review = @event.Sessions.First().IncidentReviews.First();
        var memberAtFault = @event.Sessions.First().SessionResult.ResultRows.First().Member;
        var voteCat = await dbContext.VoteCategories.FirstAsync();
        var vote = accessMockHelper.AcceptedReviewVoteBuilder()
            .With(x => x.LeagueId, review.LeagueId)
            .With(x => x.Review, review)
            .With(x => x.ReviewId, review.ReviewId)
            .With(x => x.MemberAtFault, memberAtFault)
            .With(x => x.MemberAtFaultId, memberAtFault.Id)
            .With(x => x.VoteCategory, voteCat)
            .Without(x => x.ReviewPenaltys)
            .Create();
        dbContext.AcceptedReviewVotes.Add(vote);
        await dbContext.SaveChangesAsync();
        var config = GetConfiguration(@event);
        var sut = CreateSut();

        var test = (await sut.GetData(config))!;

        test.SessionResults.First().AcceptedReviewVotes.First().ReviewVoteId.Should().Be(vote.ReviewVoteId);
    }

    [Fact]
    public async Task GetData_ShouldProvideResultFromSoure_WhenSourceConfigIsConfigured()
    {
        var @event = await GetFirstEventEntity();
        var sourceConfig = accessMockHelper.CreateConfiguration(@event);
        var sourceResult = accessMockHelper.CreateScoredResult(@event, sourceConfig);
        dbContext.ResultConfigurations.Add(sourceConfig);
        dbContext.ScoredEventResults.Add(sourceResult);
        await dbContext.SaveChangesAsync();
        var config = GetConfiguration(@event);
        config.SourceResultConfigId = sourceConfig.ResultConfigId;
        var sut = CreateSut();

        var test = await sut.GetData(config);

        test.Should().NotBeNull();
        test!.SessionResults.Should().HaveSameCount(sourceResult.ScoredSessionResults);
        foreach ((var testSession, var sourceSession) in test.SessionResults.OrderBy(x => x.SessionNr).Zip(sourceResult.ScoredSessionResults.OrderBy(x => x.SessionNr)))
        {
            foreach ((var testRow, var sourceRow) in testSession.ResultRows.OrderBy(x => x.MemberId).Zip(sourceSession.ScoredResultRows.OrderBy(x => x.MemberId)))
            {
                testRow.RacePoints.Should().Be(sourceRow.RacePoints);
                testRow.Car.Should().Be(sourceRow.Car);
            }
        }
    }

    [Fact]
    public async Task GetData_ShouldProvideAddPenalties()
    {
        var @event = await GetFirstEventEntity();
        var rawResult = @event.EventResult;
        var configEntity = accessMockHelper.CreateConfiguration(@event);
        var scoredResult = accessMockHelper.CreateScoredResult(@event, configEntity);
        var addPenalty = new AddPenaltyEntity()
        {
            LeagueId = scoredResult.LeagueId,
            ScoredResultRow = scoredResult.ScoredSessionResults.First().ScoredResultRows.First(),
            Value = new()
            {
                Type = PenaltyType.Points,
                Points = 420,
            },
        };
        dbContext.ResultConfigurations.Add(configEntity);
        dbContext.ScoredEventResults.Add(scoredResult);
        dbContext.AddPenaltys.Add(addPenalty);
        await dbContext.SaveChangesAsync();
        var config = GetConfiguration(@event);
        config.ResultId = scoredResult.ResultId;
        var sut = CreateSut();

        var test = (await sut.GetData(config))!;

        test.Should().NotBeNull();
        test.AddPenalties.Should().NotBeEmpty();
        var testPenalty = test.AddPenalties.First();
        testPenalty.SessionNr.Should().Be(scoredResult.ScoredSessionResults.First().SessionNr);
        testPenalty.MemberId.Should().Be(addPenalty.ScoredResultRow.MemberId);
        testPenalty.Type.Should().Be(addPenalty.Value.Type);
        testPenalty.Points.Should().Be(addPenalty.Value.Points);
    }

    [Fact]
    public async Task GetData_ShouldProvideResultRowData()
    {
        var @event = await GetFirstEventEntity();
        var rawResult = @event.EventResult;
        var config = GetConfiguration(@event);
        var sut = CreateSut();

        var test = (await sut.GetData(config))!;

        test.Should().NotBeNull();
        var compareRow = rawResult.SessionResults.First().ResultRows.First();
        var testRow = test.SessionResults
            .FirstOrDefault()?
            .ResultRows
            .FirstOrDefault();
        testRow.Should().NotBeNull();

        testRow!.AvgLapTime.Should().Be(compareRow.AvgLapTime);
        testRow.Car.Should().Be(compareRow.Car);
        testRow.MemberId.Should().Be(compareRow.MemberId);
        testRow.Firstname.Should().Be(compareRow.Member.Firstname);
        testRow.Lastname.Should().Be(compareRow.Member.Lastname);
        testRow.TeamId.Should().Be(compareRow.TeamId);
        testRow.TeamName.Should().Be(compareRow.Team?.Name ?? string.Empty);
        testRow.TeamColor.Should().Be(compareRow.Team?.TeamColor ?? string.Empty);
        testRow.StartPosition.Should().Be(compareRow.StartPosition);
        testRow.FinishPosition.Should().Be(compareRow.FinishPosition);
        testRow.CarNumber.Should().Be(compareRow.CarNumber);
        testRow.ClassId.Should().Be(compareRow.ClassId);
        testRow.ClubId.Should().Be(compareRow.ClubId);
        testRow.ClubName.Should().Be(compareRow.ClubName);
        testRow.Car.Should().Be(compareRow.Car);
        testRow.CarClass.Should().Be(compareRow.CarClass);
        testRow.CompletedLaps.Should().Be(compareRow.CompletedLaps);
        testRow.LeadLaps.Should().Be(compareRow.LeadLaps);
        testRow.FastLapNr.Should().Be(compareRow.FastLapNr);
        testRow.Incidents.Should().Be(compareRow.Incidents);
        testRow.Status.Should().Be(compareRow.Status);
        testRow.QualifyingTime.Should().Be(compareRow.QualifyingTime);
        testRow.Interval.Should().Be(compareRow.Interval);
        testRow.AvgLapTime.Should().Be(compareRow.AvgLapTime);
        testRow.FastestLapTime.Should().Be(compareRow.FastestLapTime);
        testRow.PositionChange.Should().Be(compareRow.PositionChange);
        testRow.OldIrating.Should().Be(compareRow.OldIRating);
        testRow.NewIrating.Should().Be(compareRow.NewIRating);
        testRow.SeasonStartIrating.Should().Be(compareRow.SeasonStartIRating);
        testRow.License.Should().Be(compareRow.License);
        testRow.NewCpi.Should().Be(compareRow.NewCpi);
        testRow.OldCpi.Should().Be(compareRow.OldCpi);
        testRow.OldSafetyRating.Should().Be(compareRow.OldSafetyRating);
        testRow.NewSafetyRating.Should().Be(compareRow.NewSafetyRating);
        testRow.CarId.Should().Be(compareRow.CarId);
        testRow.CompletedPct.Should().Be(compareRow.CompletedPct);
        testRow.Division.Should().Be(compareRow.Division);
        testRow.OldLicenseLevel.Should().Be(compareRow.OldLicenseLevel);
        testRow.NewLicenseLevel.Should().Be(compareRow.NewLicenseLevel);
        testRow.RacePoints.Should().Be(compareRow.RacePoints);
        testRow.PointsEligible.Should().Be(compareRow.PointsEligible);
    }

    [Fact]
    public async Task GetData_ShouldProvideSessionLapTimes()
    {
        var @event = await GetFirstEventEntity();
        var fastestLap = fixture.Create<TimeSpan>();
        var fastestQualyLap = fixture.Create<TimeSpan>();
        var fastestAvgLap = fixture.Create<TimeSpan>();
        var rawResult = @event.EventResult;
        foreach(var session in rawResult.SessionResults)
        {
            session.ResultRows.ForEach(x =>
            {
                x.FastestLapTime = fastestLap + fixture.Create<TimeSpan>();
                x.QualifyingTime = fastestQualyLap + fixture.Create<TimeSpan>();
                x.AvgLapTime = fastestAvgLap + fixture.Create<TimeSpan>();
            });
            session.ResultRows.GetRandom().FastestLapTime = TimeSpan.Zero;
            session.ResultRows.GetRandom().FastestLapTime = fastestLap;
            session.ResultRows.GetRandom().QualifyingTime = TimeSpan.Zero;
            session.ResultRows.GetRandom().QualifyingTime = fastestQualyLap;
            session.ResultRows.GetRandom().AvgLapTime = TimeSpan.Zero;
            session.ResultRows.GetRandom().AvgLapTime = fastestAvgLap;
        }
        await dbContext.SaveChangesAsync();
        var config = GetConfiguration(@event);
        var sut = CreateSut();

        var test = (await sut.GetData(config))!;

        test.Should().NotBeNull();
        foreach(var session in test.SessionResults)
        {
            session.FastestLap.Should().Be(fastestLap);
            session.FastestQualyLap.Should().Be(fastestQualyLap);
            session.FastestAvgLap.Should().Be(fastestAvgLap);
        }
    }

    private EventCalculationDataProvider CreateSut()
    {
        return fixture.Create<EventCalculationDataProvider>();
    }

    private async Task<EventEntity> GetFirstEventEntity()
    {
        return await dbContext.Events
            .Include(x => x.EventResult)
                .ThenInclude(x => x.SessionResults)
                    .ThenInclude(x => x.ResultRows)
            .Include(x => x.Schedule.Season.League)
            .Include(x => x.Sessions)
                .ThenInclude(x => x.IncidentReviews)
            .FirstAsync();
    }

    private EventCalculationConfiguration GetConfiguration(EventEntity @event)
    {
        return fixture.Build<EventCalculationConfiguration>()
            .With(x => x.LeagueId, @event.LeagueId)
            .With(x => x.EventId, @event.EventId)
            .Without(x => x.SourceResultConfigId)
            .Create();
    }
}
