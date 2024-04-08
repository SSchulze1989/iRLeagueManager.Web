using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Server.Models.Payments;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.AdminPanel;

public class AdminHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public AdminHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators)
        : base(logger, dbContext, validators)
    {
    }

    protected virtual LeagueEntity UpdateLeagueSubscriptionStatus(LeagueEntity league, PaymentEntity payment)
    {
        if (league.Subscription == SubscriptionStatus.Lifetime)
        {
            return league;
        }
        league.Subscription = payment.Status == PaymentStatus.Active ? SubscriptionStatus.PaidPlan : SubscriptionStatus.Expired;
        league.Expires = payment.NextPaymentDue;
        return league;
    }

    protected static Expression<Func<PaymentEntity, PaymentModel>> MapToPaymentModelExpression => payment =>
        new(payment.Id,
            payment.Type,
            payment.PlanId ?? string.Empty,
            payment.Subscription != null ? payment.Subscription.Name : string.Empty,
            payment.Subscription != null ? payment.Subscription.Interval : default,
            payment.SubscriptionId ?? string.Empty,
            payment.LeagueId,
            payment.UserId,
            payment.LastPaymentReceived,
            payment.NextPaymentDue,
            payment.Status);
}
