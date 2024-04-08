using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Seasons;

public sealed class PutSeasonDbTestFixture : HandlersTestsBase<PutSeasonHandler, PutSeasonRequest, SeasonModel>
{
    private const string testSeasonName = "TestSeason";

    public PutSeasonDbTestFixture() : base()
    {
    }

    protected override PutSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PutSeasonRequest> validator)
    {
        return new PutSeasonHandler(logger, dbContext, new IValidator<PutSeasonRequest>[] { validator });
    }

    protected override PutSeasonRequest DefaultRequest()
    {
        return DefaultRequest(TestSeasonId);
    }

    private PutSeasonRequest DefaultRequest(long seasonId)
    {
        var model = new PutSeasonModel()
        {
            SeasonName = testSeasonName
        };
        return new PutSeasonRequest(DefaultUser(), seasonId, model);
    }

    protected override void DefaultPreTestAssertions(PutSeasonRequest request, LeagueDbContext dbContext)
    {
        Assert.NotEqual(dbContext.Seasons.Single(x => x.SeasonId == request.SeasonId).SeasonName, request.Model.SeasonName);
    }

    protected override void DefaultAssertions(PutSeasonRequest request, SeasonModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.LeagueId.Should().Be(accessMockHelper.LeagueProvider.LeagueId);
        result.SeasonId.Should().Be(request.SeasonId);
        result.Finished.Should().Be(expected.Finished);
        result.HideComments.Should().Be(expected.HideComments);
        result.MainScoringId.Should().Be(expected.MainScoringId);
        result.SeasonName.Should().Be(expected.SeasonName);
        AssertChanged(request.User, DateTime.UtcNow, result);
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
