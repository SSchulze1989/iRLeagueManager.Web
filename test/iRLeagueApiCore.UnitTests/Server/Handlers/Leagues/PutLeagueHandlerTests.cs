using FluentValidation;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.UnitTests.Fixtures;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.Leagues;

public sealed class PutLeagueDbTestFixture : HandlersTestsBase<PutLeagueHandler, PutLeagueRequest, LeagueModel>
{
    public PutLeagueDbTestFixture() : base()
    {
    }

    protected override PutLeagueHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PutLeagueRequest> validator)
    {
        return new PutLeagueHandler(logger, dbContext, new IValidator<PutLeagueRequest>[] { validator });
    }

    protected override PutLeagueRequest DefaultRequest()
    {
        return DefaultRequest(dbContext.Leagues.First().Id);
    }

    public PutLeagueRequest DefaultRequest(long leagueId)
    {
        var model = new PutLeagueModel()
        {
            NameFull = "Put league test",
            EnableProtests = true,
            ProtestCoolDownPeriod = fixture.Create<TimeSpan>(),
            ProtestsClosedAfter = fixture.Create<TimeSpan>(),
            ProtestsPublic = fixture.Create<ProtestPublicSetting>(),
        };
        return new PutLeagueRequest(leagueId, DefaultUser(), model);
    }

    protected override void DefaultAssertions(PutLeagueRequest request, LeagueModel result, LeagueDbContext dbContext)
    {
        var expected = request.Model;
        result.Id.Should().Be(request.LeagueId);
        result.NameFull.Should().Be(expected.NameFull);
        result.EnableProtests.Should().Be(request.Model.EnableProtests);
        result.ProtestCoolDownPeriod.Should().Be(request.Model.ProtestCoolDownPeriod);
        result.ProtestsClosedAfter.Should().Be(request.Model.ProtestsClosedAfter);
        result.ProtestsPublic.Should().Be(request.Model.ProtestsPublic);
        AssertChanged(request.User, DateTime.UtcNow, result);
        base.DefaultAssertions(request, result, dbContext);
    }

    [Fact]
    public override async Task<LeagueModel> ShouldHandleDefault()
    {
        return await base.ShouldHandleDefault();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-42)]
    public async Task HandleNotFoundAsync(long leagueId)
    {
        var request = DefaultRequest(leagueId);
        await base.HandleNotFoundRequestAsync(request);
    }

    [Fact]
    public async override Task ShouldHandleValidationFailed()
    {
        await base.ShouldHandleValidationFailed();
    }
}
