using FluentValidation;
using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Server.Handlers.AdminPanel;
using iRLeagueApiCore.Server.Models.Payments;
using iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;

public sealed class PostPaymentHandlerTests
    : AdminPanelTestsBase<PostPaymentHandler, PostPaymentRequest, PaymentModel>
{
    protected override PostPaymentHandler CreateTestHandler(LeagueDbContext dbContext, IValidator<PostPaymentRequest> validator)
    {
        return new(logger, dbContext, new[] { validator });
    }

    public static IEnumerable<object?[]> GetSetPaymentStatusData()
    {
        yield return new object?[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1), PaymentStatus.Active };
        yield return new object?[] { DateTime.UtcNow, null, PaymentStatus.Active };
        yield return new object?[] { DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1), PaymentStatus.Inactive };
    }

    protected override PostPaymentRequest DefaultRequest()
    {
        var model = fixture.Build<PostPaymentModel>()
            .With(x => x.PlanId, dbContext.Subscriptions.Last().PlanId)
            .Create();
        return new(TestLeagueId, model);
    }

    private static PostPaymentRequest DefaultRequest(long leagueId, PostPaymentModel model)
    {
        return new(leagueId, model);
    }

    protected override void DefaultAssertions(PostPaymentRequest request, PaymentModel result, LeagueDbContext dbContext)
    {
        base.DefaultAssertions(request, result, dbContext);
        var payment = dbContext.Payments
            .Include(x => x.Subscription)
            .First(x => x.Id == result.Id);
        payment.Subscription.Should().NotBeNull();

        result.LeagueId.Should().Be(request.LeagueId);
        result.PlanId.Should().Be(request.Model.PlanId);
        result.Interval.Should().Be(payment.Subscription!.Interval);
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
    [MemberData(nameof(GetSetPaymentStatusData))]

    public async Task Handle_ShouldSetPaymentStatus(DateTime received, DateTime? due, PaymentStatus expected)
    {
        var model = fixture.Build<PostPaymentModel>()
            .With(x => x.Received, received)
            .With(x => x.NextDue, due)
            .Create();
        var request = DefaultRequest(TestLeagueId, model);
        var sut = CreateTestHandler(dbContext, DefaultValidator());

        var test = await sut.Handle(request, default);

        test.Status.Should().Be(expected);
    }

    [Theory]
    [InlineData(SubscriptionStatus.Expired, SubscriptionStatus.PaidPlan)]
    [InlineData(SubscriptionStatus.Unknown, SubscriptionStatus.PaidPlan)]
    [InlineData(SubscriptionStatus.FreeTrial, SubscriptionStatus.PaidPlan)]
    [InlineData(SubscriptionStatus.PaidPlan, SubscriptionStatus.PaidPlan)]
    [InlineData(SubscriptionStatus.Lifetime, SubscriptionStatus.Lifetime)]
    public async Task Handle_ShouldSetLeagueSubscriptionStatus(SubscriptionStatus before, SubscriptionStatus after)
    {
        var model = fixture.Build<PostPaymentModel>()
            .With(x => x.Received, DateTime.UtcNow)
            .With(x => x.NextDue, DateTime.UtcNow.AddMonths(1))
            .Create();
        var request = DefaultRequest(TestLeagueId, model);
        var league = await dbContext.Leagues.FirstAsync(x => x.Id == request.LeagueId);
        league.Subscription = before;
        await dbContext.SaveChangesAsync();
        var sut = CreateTestHandler(dbContext, DefaultValidator());

        var test = await sut.Handle(request, default);

        league = await dbContext.Leagues.FirstAsync(x => x.Id == request.LeagueId);
        league.Subscription.Should().Be(after);
        if (after != SubscriptionStatus.Lifetime)
        {
            league.Expires.Should().Be(request.Model.NextDue);
        }
    }
}
