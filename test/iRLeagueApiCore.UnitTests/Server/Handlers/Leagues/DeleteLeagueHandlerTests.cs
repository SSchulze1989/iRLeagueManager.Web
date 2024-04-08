using FluentValidation;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using MediatR;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;

public sealed class DeleteLeagueDbTestFixture : HandlersTestsBase<DeleteLeagueHandler, DeleteLeagueRequest, Unit>
{
    public DeleteLeagueDbTestFixture() : base()
    {
    }

    protected override DeleteLeagueHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<DeleteLeagueRequest> validator)
    {
        return new DeleteLeagueHandler(logger, dbContext, new IValidator<DeleteLeagueRequest>[] { validator });
    }

    protected override DeleteLeagueRequest DefaultRequest()
    {
        return DefaultRequest(dbContext.Leagues.First().Id);
    }

    protected override void DefaultPreTestAssertions(DeleteLeagueRequest request, LeagueDbContext dbContext)
    {
        Assert.Contains(dbContext.Leagues, x => x.Id == request.LeagueId);
    }

    protected override void DefaultAssertions(DeleteLeagueRequest request, Unit result, LeagueDbContext dbContext)
    {
        Assert.DoesNotContain(dbContext.Leagues, x => x.Id == request.LeagueId);
    }

    private DeleteLeagueRequest DefaultRequest(long leagueId)
    {
        return new DeleteLeagueRequest(leagueId);
    }

    [Fact]
    public async override Task<Unit> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public async override Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-42)]
    public async Task HandleNotFoundAsync(long leagueId)
    {
        var request = DefaultRequest(leagueId);
        await base.HandleNotFoundRequestAsync(request);
    }
}
