using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Seasons;

public sealed class GetSeasonDbTestFixture : HandlersTestsBase<GetSeasonHandler, GetSeasonRequest, SeasonModel>
{
    public GetSeasonDbTestFixture() : base()
    {
    }

    protected override GetSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetSeasonRequest> validator)
    {
        return new GetSeasonHandler(logger, dbContext, new IValidator<GetSeasonRequest>[] { validator });
    }

    protected override GetSeasonRequest DefaultRequest()
    {
        return DefaultRequest(TestSeasonId);
    }

    private GetSeasonRequest DefaultRequest(long seasonId)
    {
        return new GetSeasonRequest(seasonId);
    }

    protected override void DefaultAssertions(GetSeasonRequest request, SeasonModel result, LeagueDbContext dbContext)
    {
        var testSeason = dbContext.Seasons
            .Include(x => x.Schedules)
            .First(x => x.SeasonId == request.SeasonId);
        result.SeasonId.Should().Be(request.SeasonId);
        result.Finished.Should().Be(testSeason.Finished);
        result.HideComments.Should().Be(testSeason.HideCommentsBeforeVoted);
        result.ScheduleIds.Should().BeEquivalentTo(testSeason.Schedules.Select(x => x.ScheduleId));
        result.SeasonEnd.Should().Be(testSeason.Schedules.SelectMany(x => x.Events)
            .OrderBy(x => x.Date)
            .Last().Date);
        result.SeasonStart.Should().Be(testSeason.Schedules.SelectMany(x => x.Events)
            .OrderBy(x => x.Date)
            .First().Date);
        result.SeasonName.Should().Be(testSeason.SeasonName);
        AssertVersion(testSeason, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public async override Task<SeasonModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -42)]
    [InlineData(-42, 1)]
    public async Task HandleNotFound(long leagueId, long seasonId)
    {
        accessMockHelper.SetCurrentLeague(leagueId);
        var request = DefaultRequest(seasonId);
        await HandleNotFoundRequestAsync(request);
    }
}
