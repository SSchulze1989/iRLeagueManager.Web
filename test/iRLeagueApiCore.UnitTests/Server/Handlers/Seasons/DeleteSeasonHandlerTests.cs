
using FluentValidation;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using MediatR;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Seasons;

public sealed class DeleteSeasonDbTestFixture : HandlersTestsBase<DeleteSeasonHandler, DeleteSeasonRequest, Unit>
{
    private const string testSeasonName = "TestSeason";

    public DeleteSeasonDbTestFixture() : base()
    {
    }

    protected override DeleteSeasonHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<DeleteSeasonRequest> validator)
    {
        return new DeleteSeasonHandler(logger, dbContext, new IValidator<DeleteSeasonRequest>[] { validator });
    }

    protected override DeleteSeasonRequest DefaultRequest()
    {
        return DefaultRequest(TestSeasonId);
    }

    private DeleteSeasonRequest DefaultRequest(long seasonId)
    {

        return new DeleteSeasonRequest(seasonId);
    }

    protected override void DefaultPreTestAssertions(DeleteSeasonRequest request, LeagueDbContext dbContext)
    {
        Assert.Contains(dbContext.Seasons, x => x.SeasonId == request.SeasonId);
    }

    protected override void DefaultAssertions(DeleteSeasonRequest request, Unit result, LeagueDbContext dbContext)
    {
        Assert.DoesNotContain(dbContext.Seasons, x => x.SeasonId == request.SeasonId);
    }

    [Fact]
    public async override Task<Unit> ShouldHandleDefault()
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
    [InlineData(-42, 1)]
    [InlineData(1, -42)]
    public async Task HandleNotFound(long leagueId, long seasonId)
    {
        accessMockHelper.SetCurrentLeague(leagueId);
        var request = DefaultRequest(seasonId);
        await HandleNotFoundRequestAsync(request);
    }
}
