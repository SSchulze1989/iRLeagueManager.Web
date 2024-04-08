using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;

public sealed class GetLeaguesDbTestFixture : HandlersTestsBase<GetLeaguesHandler, GetLeaguesRequest, IEnumerable<LeagueModel>>
{
    public GetLeaguesDbTestFixture() : base()
    {
    }

    protected override GetLeaguesHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetLeaguesRequest> validator)
    {
        return new GetLeaguesHandler(logger, dbContext, new IValidator<GetLeaguesRequest>[] { validator });
    }

    protected override GetLeaguesRequest DefaultRequest()
    {
        return new GetLeaguesRequest(Array.Empty<string>());
    }

    [Fact]
    public override async Task<IEnumerable<LeagueModel>> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Fact]
    public override async Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }

    [Fact]
    public async Task ShouldReturnPublicListedLeaguesOnly()
    {
        var privateLeague = accessMockHelper.CreateLeague();
        privateLeague.LeaguePublic = Common.Enums.LeaguePublicSetting.PublicHidden;
        dbContext.Leagues.Add(privateLeague);
        await dbContext.SaveChangesAsync();
        var sut = CreateTestHandler(dbContext, MockHelpers.TestValidator<GetLeaguesRequest>());

        var test = await sut.Handle(DefaultRequest(), default);

        test.Should().AllSatisfy(x => x.Id.Should().NotBe(privateLeague.Id));
    }

    [Fact]
    public async Task ShouldReturnHiddenLeague_IfUserIsMember()
    {
        var privateLeague = accessMockHelper.CreateLeague();
        privateLeague.LeaguePublic = Common.Enums.LeaguePublicSetting.PublicHidden;
        dbContext.Leagues.Add(privateLeague);
        await dbContext.SaveChangesAsync();
        var sut = CreateTestHandler(dbContext, MockHelpers.TestValidator<GetLeaguesRequest>());

        var test = await sut.Handle(new GetLeaguesRequest(new[] { privateLeague.Name }), default);

        test.Should().Contain(x => x.Name == privateLeague.Name);
    }
}
