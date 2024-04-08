using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetPenaltiesFromEventResultRequest(long ResultId) : IRequest<IEnumerable<PenaltyModel>>;

public class GetPenaltiesFromEventResultHandler : ReviewsHandlerBase<GetPenaltiesFromEventResultHandler, GetPenaltiesFromEventResultRequest>, 
    IRequestHandler<GetPenaltiesFromEventResultRequest, IEnumerable<PenaltyModel>>
{
    public GetPenaltiesFromEventResultHandler(ILogger<GetPenaltiesFromEventResultHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetPenaltiesFromEventResultRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<PenaltyModel>> Handle(GetPenaltiesFromEventResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getPenalties = await GetPenaltiesFromEventResult(request.ResultId, cancellationToken);
        return getPenalties;
    }

    private async Task<IEnumerable<PenaltyModel>> GetPenaltiesFromEventResult(long resultId, CancellationToken cancellationToken)
    {
        var addPenalties = await dbContext.AddPenaltys
            .Where(x => x.ScoredResultRow.ScoredSessionResult.ResultId == resultId)
            .Select(MapToAddPenaltyModelExpression)
            .ToListAsync(cancellationToken);
        var reviewPenalties = await dbContext.ReviewPenaltys
            .Where(x => x.ResultRow.ScoredSessionResult.ResultId == resultId)
            .Select(MapToReviewPenaltyModelExpression)
            .ToListAsync(cancellationToken);
        return addPenalties.Concat(reviewPenalties);
    }
}
