using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;
public sealed class PostInitializeLeagueHandlerTests : HandlersTestsBase<PostInitializeLeagueHandler, PostIntitializeLeagueRequest, LeagueModel>
{
    protected override PostInitializeLeagueHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostIntitializeLeagueRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override PostIntitializeLeagueRequest DefaultRequest()
    {
        return DefaultRequest(TestLeagueId);
    }

    private PostIntitializeLeagueRequest DefaultRequest(long leagueId)
    {
        return new(leagueId);
    }

    [Fact]
    public async Task ShouldSetInitializedTrue()
    {
        var testLeague = await dbContext.Leagues.FirstAsync();
        testLeague.IsInitialized = false;
        await dbContext.SaveChangesAsync();
        var request = DefaultRequest(testLeague.Id);
        var sut = CreateTestHandler(dbContext, DefaultValidator());

        await sut.Handle(request, default);

        testLeague.IsInitialized.Should().BeTrue();
    }
}
