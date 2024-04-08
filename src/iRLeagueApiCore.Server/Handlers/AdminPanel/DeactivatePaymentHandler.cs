using iRLeagueApiCore.Server.Models.Payments;

namespace iRLeagueApiCore.Server.Handlers.AdminPanel;

public record DeactivatePaymentRequest(Guid Id) : IRequest<PaymentModel>;

public class DeactivatePaymentHandler : AdminHandlerBase<DeactivatePaymentHandler, DeactivatePaymentRequest>, 
    IRequestHandler<DeactivatePaymentRequest, PaymentModel>
{
    public DeactivatePaymentHandler(ILogger<DeactivatePaymentHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<DeactivatePaymentRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PaymentModel> Handle(DeactivatePaymentRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var payment = await dbContext.Payments
            .Include(x => x.League)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new ResourceNotFoundException();
        payment.Status = PaymentStatus.Inactive;
        payment.League = UpdateLeagueSubscriptionStatus(payment.League!, payment);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPayment = await dbContext.Payments
            .Where(x => x.Id == request.Id)
            .Select(MapToPaymentModelExpression)
            .FirstAsync(cancellationToken);
        return getPayment;
    }
}
