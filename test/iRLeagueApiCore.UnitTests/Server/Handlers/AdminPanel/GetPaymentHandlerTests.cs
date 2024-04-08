using FluentValidation;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using iRLeagueDatabaseCore.Models;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
public sealed class GetPaymentHandlerTests
    : AdminPanelTestsBase<GetPaymentHandler, GetPaymentRequest, PaymentModel>
{
    protected override GetPaymentRequest DefaultRequest()
    {
        return DefaultRequest(TestPaymentId);
    }

    private GetPaymentRequest DefaultRequest(Guid paymentId)
    {
        return new(paymentId);
    }

    protected override GetPaymentHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<GetPaymentRequest> validator)
    {
        return new GetPaymentHandler(logger, dbContext, new[] { validator });
    }

    protected override void DefaultAssertions(GetPaymentRequest request, PaymentModel result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        var payment = dbContext.Payments
            .Include(x => x.Subscription)
            .IgnoreQueryFilters()
            .First(x => x.Id == request.Id);
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
        result.Interval.Should().Be(payment.Subscription!.Interval);
        result.Status.Should().Be(payment.Status);
        result.SubscriptionId.Should().Be(payment.SubscriptionId);
        result.LastPayment.Should().Be(payment.LastPaymentReceived);
        result.NextPayment.Should().Be(payment.NextPaymentDue);
        result.UserId.Should().Be(payment.UserId);
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
}
