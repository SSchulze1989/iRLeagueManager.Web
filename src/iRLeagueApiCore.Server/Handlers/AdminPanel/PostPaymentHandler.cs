using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Server.Models.Payments;

namespace iRLeagueApiCore.Server.Handlers.AdminPanel;

public record PostPaymentRequest(long LeagueId, PostPaymentModel Model) : IRequest<PaymentModel>;

public sealed class PostPaymentHandler : AdminHandlerBase<PostPaymentHandler, PostPaymentRequest>,
    IRequestHandler<PostPaymentRequest, PaymentModel>
{
    public PostPaymentHandler(ILogger<PostPaymentHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<PostPaymentRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PaymentModel> Handle(PostPaymentRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var league = await dbContext.Leagues.FirstAsync(x => x.Id == request.LeagueId, cancellationToken);
        var postPayment = await CreatePayment(league, request.Model, cancellationToken);
        league = UpdateLeagueSubscriptionStatus(league, postPayment);
        dbContext.Payments.Add(postPayment);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPayment = await dbContext.Payments
            .Where(x => x.Id == postPayment.Id)
            .Select(MapToPaymentModelExpression)
            .FirstAsync(cancellationToken);
        return getPayment;
    }

    private async Task<PaymentEntity> CreatePayment(LeagueEntity league, PostPaymentModel model, CancellationToken cancellationToken)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.PlanId == model.PlanId, cancellationToken);
        var payment = new PaymentEntity()
        {
            LastPaymentReceived = model.Received,
            League = league,
            LeagueId = league.Id,
            NextPaymentDue = model.NextDue,
            PlanId = model.PlanId,
            Subscription = subscription,
            SubscriptionId = model.SubscriptionId,
            Status = (model.NextDue == null || model.NextDue > DateTime.UtcNow) ? PaymentStatus.Active : PaymentStatus.Inactive,
            Type = model.PaymentType,
            UserId = model.UserId,
        };
        return payment;
    }
}
