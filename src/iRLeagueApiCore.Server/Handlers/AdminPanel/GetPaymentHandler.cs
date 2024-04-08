using iRLeagueApiCore.Server.Models.Payments;

namespace iRLeagueApiCore.Server.Handlers.AdminPanel;

public record GetPaymentRequest(Guid Id) : IRequest<PaymentModel>;

public class GetPaymentHandler : AdminHandlerBase<GetPaymentHandler, GetPaymentRequest>, IRequestHandler<GetPaymentRequest, PaymentModel>
{
    public GetPaymentHandler(ILogger<GetPaymentHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetPaymentRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PaymentModel> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getPayment = await dbContext.Payments
            .Select(MapToPaymentModelExpression)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getPayment;
    }
}
