using AutoFixture.Dsl;
using iRLeagueApiCore.Mocking.DataAccess;
using iRLeagueDatabaseCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.UnitTests.Server.Handlers.AdminPanel;
public abstract class AdminPanelTestsBase<THandler, TRequest, TResult> 
    : HandlersTestsBase<THandler, TRequest, TResult>
    where THandler : IRequestHandler<TRequest, TResult>
    where TRequest : class, IRequest<TResult>
{

    protected Guid TestPaymentId => dbContext.Payments.IgnoreQueryFilters().First().Id;
    protected string TestPlanId => dbContext.Subscriptions.IgnoreQueryFilters().First().PlanId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var subscriptions = await CreateTestSubscriptions();
        dbContext.Subscriptions.AddRange(subscriptions);
        var payments = await CreateTestPayments(subscriptions);
        dbContext.Payments.AddRange(payments);
        await dbContext.SaveChangesAsync();
    }

    private async Task<IEnumerable<PaymentEntity>> CreateTestPayments(IEnumerable<SubscriptionEntity> subscriptions)
    {
        var league = await dbContext.Leagues.FirstAsync();
        var payment1 = PaymentEntityBuilder(league, subscriptions.First())
            .With(x => x.LastPaymentReceived, DateTime.UtcNow)
            .With(x => x.LastPaymentReceived, DateTime.UtcNow.AddMonths(1))
            .Create();
        var payment2 = PaymentEntityBuilder(league, subscriptions.Last())
            .With(x => x.LastPaymentReceived, DateTime.UtcNow)
            .With(x => x.LastPaymentReceived, DateTime.UtcNow.AddYears(1))
            .Create();
        return new[] { payment1, payment2 };
    }

    private async Task<IEnumerable<SubscriptionEntity>> CreateTestSubscriptions()
    {
        var monthlySub = SubscriptionEntityBuilder()
            .With(x => x.Name, "Monthly")
            .With(x => x.Interval, SubscriptionInterval.Monthly)
            .Create();
        var yearlySub = SubscriptionEntityBuilder()
            .With(x => x.Name, "Yealy")
            .With(x => x.Interval, SubscriptionInterval.Yearly)
            .Create();
        return await Task.FromResult(new[] { monthlySub, yearlySub });
    }

    protected IPostprocessComposer<SubscriptionEntity> SubscriptionEntityBuilder()
    {
        return fixture.Build<SubscriptionEntity>()
            .Without(x => x.Payments);
    }

    protected IPostprocessComposer<PaymentEntity> PaymentEntityBuilder(LeagueEntity league, SubscriptionEntity? subscription)
    {
        return fixture.Build<PaymentEntity>()
            .With(x => x.League, league)
            .With(x => x.LeagueId, league.Id)
            .With(x => x.Subscription, subscription)
            .With(x => x.PlanId, subscription?.PlanId);
    }
}
