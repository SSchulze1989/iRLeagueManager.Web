using FluentValidation;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Results;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Results;

public sealed class GetResultHandlerTests : ResultHandlersTestsBase<GetResultHandler, GetResultRequest, EventResultModel>
{
    public GetResultHandlerTests() : base()
    {
    }

    protected override GetResultHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetResultRequest> validator)
    {
        return new GetResultHandler(logger, dbContext, new IValidator<GetResultRequest>[] { validator });
    }

    protected override GetResultRequest DefaultRequest()
    {
        return DefaultRequest(TestResultId);
    }

    private GetResultRequest DefaultRequest(long resultId)
    {
        return new GetResultRequest(resultId);
    }

    protected override void DefaultAssertions(GetResultRequest request, EventResultModel result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        var actualResult = dbContext.ScoredEventResults
            .Where(x => x.ResultId == request.ResultId)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.ScoredResultRows)
                    .ThenInclude(x => x.Member)
            .Include(x => x.ScoredSessionResults)
                .ThenInclude(x => x.ScoredResultRows)
                    .ThenInclude(x => x.Team)
            .Single();
        AssertEventResultData(result, actualResult);
    }

    [Fact]
    public async override Task<EventResultModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public async override Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0, defaultId)]
    [InlineData(defaultId, 0)]
    [InlineData(-42, defaultId)]
    [InlineData(defaultId, -42)]
    public async Task HandleNotFoundAsync(long? leagueId, long? resultId)
    {
        leagueId ??= TestLeagueId;
        resultId ??= TestResultId;
        accessMockHelper.SetCurrentLeague(leagueId.Value);
        var request = DefaultRequest(resultId.Value);
        await HandleNotFoundRequestAsync(request);
    }

    private void AssertEventResultData(EventResultModel expected, ScoredEventResultEntity test)
    {
        Assert.Equal(expected.LeagueId, test.LeagueId);
        AssertSessionResultData(expected.SessionResults.OrderBy(x => x.SessionNr).First(), test.ScoredSessionResults.OrderBy(x => x.SessionNr).First());
    }

    private void AssertSessionResultData(ResultModel expected, ScoredSessionResultEntity test)
    {
        Assert.Equal(expected.ScoringId, test.ScoringId);
        Assert.Equal(expected.LeagueId, test.LeagueId);
        AssertResultRowData(expected.ResultRows.OrderBy(x => x.FinalPosition).First(), test.ScoredResultRows.OrderBy(x => x.FinalPosition).First());
    }

    private void AssertResultRowData(ResultRowModel expected, ScoredResultRowEntity test)
    {
        Assert.Equal(expected.BonusPoints, test.BonusPoints);
        Assert.Equal(expected.FinalPosition, test.FinalPosition);
        Assert.Equal(expected.FinalPositionChange, test.FinalPositionChange);
        Assert.Equal(expected.PenaltyPoints, test.PenaltyPoints);
        Assert.Equal(expected.RacePoints, test.RacePoints);
        Assert.Equal(expected.AvgLapTime, test.AvgLapTime);
        Assert.Equal(expected.Car, test.Car);
        Assert.Equal(expected.CarClass, test.CarClass);
        Assert.Equal(expected.CarId, test.CarId);
        Assert.Equal(expected.CarNumber, test.CarNumber);
        Assert.Equal(expected.ClassId, test.ClassId);
        Assert.Equal(expected.CompletedLaps, test.CompletedLaps);
        Assert.Equal(expected.CompletedPct, test.CompletedPct);
        Assert.Equal(expected.Division, test.Division);
        Assert.Equal(expected.FastestLapTime, test.FastestLapTime);
        Assert.Equal(expected.FastLapNr, test.FastLapNr);
        Assert.Equal(expected.FinishPosition, test.FinishPosition);
        Assert.Equal(expected.Incidents, test.Incidents);
        Assert.Equal(expected.Interval, new Interval(test.Interval));
        Assert.Equal(expected.LeadLaps, test.LeadLaps);
        Assert.Equal(expected.License, test.License);
        Assert.Equal(expected.Firstname, test.Member?.Firstname ?? string.Empty);
        Assert.Equal(expected.Lastname, test.Member?.Lastname ?? string.Empty);
        Assert.Equal(expected.MemberId, test.MemberId);
        Assert.Equal(expected.NewIrating, test.NewIRating);
        Assert.Equal(expected.NewLicenseLevel, test.NewLicenseLevel);
        Assert.Equal(expected.NewSafetyRating, test.NewSafetyRating);
        Assert.Equal(expected.OldIrating, test.OldIRating);
        Assert.Equal(expected.OldLicenseLevel, test.OldLicenseLevel);
        Assert.Equal(expected.OldSafetyRating, test.OldSafetyRating);
        Assert.Equal(expected.PositionChange, test.PositionChange);
        Assert.Equal(expected.QualifyingTime, test.QualifyingTime);
        Assert.Equal(expected.SeasonStartIrating, test.SeasonStartIRating);
        Assert.Equal(expected.StartPosition, test.StartPosition);
        Assert.Equal(expected.Status, (RaceStatus)test.Status);
        Assert.Equal(expected.TeamName, test.Team?.Name ?? string.Empty);
        Assert.Equal(expected.TeamId, test.TeamId);
        Assert.Equal(expected.TotalPoints, test.TotalPoints);
    }
}
