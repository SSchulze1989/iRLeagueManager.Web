using FluentValidation;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
public sealed class DeactivatePaymentHandlerTests
    : AdminPanelTestsBase<DeactivatePaymentHandler, DeactivatePaymentRequest, PaymentModel>
{
    protected override DeactivatePaymentHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<DeactivatePaymentRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    protected override DeactivatePaymentRequest DefaultRequest()
    {
        return DefaultRequest(TestPaymentId);
    }

    private static DeactivatePaymentRequest DefaultRequest(Guid id)
    {
        return new(id);
    }

    protected override void DefaultAssertions(DeactivatePaymentRequest request, PaymentModel result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        result.Status.Should().Be(PaymentStatus.Inactive);
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

    [Fact]
    public async Task Handle_ShouldHandleNotFound()
    {
        var id = Guid.NewGuid();
        var request = DefaultRequest(id);
        await HandleNotFoundRequestAsync(request);
    }

    [Fact]
    public async Task Handle_ShouldSetLeagueSubscriptionExpired()
    {
        var request = DefaultRequest(TestPaymentId);
        var sut = CreateTestHandler(dbContext, DefaultValidator());
        var league = await dbContext.Leagues.FirstAsync(x => x.Id == TestLeagueId);
        league.Subscription = Common.Enums.SubscriptionStatus.PaidPlan;
        await dbContext.SaveChangesAsync();

        var test = await sut.Handle(request, default);

        league = await dbContext.Leagues.FirstAsync(x => x.Id == TestLeagueId);
        league.Subscription.Should().Be(Common.Enums.SubscriptionStatus.Expired);
    }
}
