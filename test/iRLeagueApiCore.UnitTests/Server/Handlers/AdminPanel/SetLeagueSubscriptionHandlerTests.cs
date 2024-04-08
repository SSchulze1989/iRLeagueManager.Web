using FluentValidation;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
public sealed class SetLeagueSubscriptionHandlerTests
    : AdminPanelTestsBase<SetLeagueSubscriptionHandler, SetLeagueSubscriptionRequest, LeagueModel>
{
    private readonly DateTime testDateTime;

    public SetLeagueSubscriptionHandlerTests()
    {
        testDateTime = fixture.Create<DateTime>();
    }

    protected override SetLeagueSubscriptionHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<SetLeagueSubscriptionRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override SetLeagueSubscriptionRequest DefaultRequest()
    {
        return DefaultRequest(TestLeagueId);
    }

    private SetLeagueSubscriptionRequest DefaultRequest(long leagueId)
    {
        var model = new SetLeagueSubscriptionModel(Common.Enums.SubscriptionStatus.PaidPlan, testDateTime);
        return new(leagueId, model);
    }

    protected override void DefaultAssertions(SetLeagueSubscriptionRequest request, LeagueModel result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        result.SubscriptionStatus.Should().Be(request.Model.Status);
        result.SubscriptionExpires.Should().Be(request.Model.Expires);
    }

    [Fact]
    public async Task Handle_ShouldHandleDefault()
    {
        await ShouldHandleDefault();
    }

    [Fact]
    public async Task Handle_ShouldHandleValidationFailed()
    {
        await ShouldHandleValidationFailed();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-42)]
    public async Task Handle_ShouldHandleNotFound(long leagueId)
    {
        var request = DefaultRequest(leagueId);
        await HandleNotFoundRequestAsync(request);
    }
}
