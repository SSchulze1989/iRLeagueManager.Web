using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Seasons;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Seasons;

public sealed class GetSeasonsDbTestFixture : HandlersTestsBase<GetSeasonsHandler, GetSeasonsRequest, IEnumerable<SeasonModel>>
{
    public GetSeasonsDbTestFixture() : base()
    {
    }

    protected override GetSeasonsHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetSeasonsRequest> validator)
    {
        return new GetSeasonsHandler(logger, dbContext, new IValidator<GetSeasonsRequest>[] { validator });
    }

    protected override GetSeasonsRequest DefaultRequest()
    {
        return new GetSeasonsRequest();
    }

    [Fact]
    public async override Task<IEnumerable<SeasonModel>> ShouldHandleDefault()
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
    public async Task HandleNotFound(long leagueId)
    {
        accessMockHelper.SetCurrentLeague(leagueId);
        var request = DefaultRequest();
        await HandleNotFoundRequestAsync(request);
    }
}
