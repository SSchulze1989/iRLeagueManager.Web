using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record PutPenaltyRequest(long PenaltyId, PutPenaltyModel Model) : IRequest<PenaltyModel>;

public class PutPenaltyHandler : ReviewsHandlerBase<PutPenaltyHandler, PutPenaltyRequest>, 
    IRequestHandler<PutPenaltyRequest, PenaltyModel>
{
    public PutPenaltyHandler(ILogger<PutPenaltyHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutPenaltyRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PenaltyModel> Handle(PutPenaltyRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putPenalty = await GetAddPenaltyEntity(request.PenaltyId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putPenalty = await MapToAddPenaltyEntity(request.Model, putPenalty, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPenalty = await MapToAddPenaltyModel(putPenalty.AddPenaltyId, cancellationToken)
            ?? throw new InvalidOperationException("Updated resource not found");
        return getPenalty;
    }
}
