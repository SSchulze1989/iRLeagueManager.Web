using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;

public sealed class GetLeagueDbTestFixture : HandlersTestsBase<GetLeagueHandler, GetLeagueRequest, LeagueModel>
{
    public GetLeagueDbTestFixture() : base()
    {
    }

    protected override GetLeagueHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetLeagueRequest> validator)
    {
        return new GetLeagueHandler(logger, dbContext, new IValidator<GetLeagueRequest>[] { validator });
    }

    protected override GetLeagueRequest DefaultRequest()
    {
        return DefaultRequest(dbContext.Leagues.First().Id);
    }

    private GetLeagueRequest DefaultRequest(long leagueId)
    {
        return new GetLeagueRequest(leagueId, false);
    }

    protected override void DefaultAssertions(GetLeagueRequest request, LeagueModel result, LeagueDbContext dbContext)
    {
        var entity = dbContext.Leagues.First(x => x.Id == request.LeagueId);
        result.IsInitialized.Should().Be(entity.IsInitialized);
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public override async Task<LeagueModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-42)]
    public async Task HandleNotFoundAsync(long leagueId)
    {
        var request = DefaultRequest(leagueId);
        await HandleNotFoundRequestAsync(request);
    }
}
