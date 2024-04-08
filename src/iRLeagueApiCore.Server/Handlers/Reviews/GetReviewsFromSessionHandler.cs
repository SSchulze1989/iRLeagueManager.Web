using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Extensions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetReviewsFromSessionRequest(long SessionId, bool IncludeComments) : IRequest<IEnumerable<ReviewModel>>;
public sealed class GetReviewsFromSessionHandler : ReviewsHandlerBase<GetReviewsFromSessionHandler, GetReviewsFromSessionRequest>,
    IRequestHandler<GetReviewsFromSessionRequest, IEnumerable<ReviewModel>>
{
    public GetReviewsFromSessionHandler(ILogger<GetReviewsFromSessionHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetReviewsFromSessionRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ReviewModel>> Handle(GetReviewsFromSessionRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getReviews = await MapToGetReviewsFromSessionAsync(request.SessionId, request.IncludeComments, cancellationToken);
        return getReviews;
    }

    private async Task<IEnumerable<ReviewModel>> MapToGetReviewsFromSessionAsync(long sessionId, bool includeComments, CancellationToken cancellationToken)
    {
        return (await dbContext.IncidentReviews
            .Where(x => x.SessionId == sessionId)
            .Select(MapToReviewModelExpression(includeComments))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.SessionNr)
            .ThenBy(x => x.IncidentNr.PadNumbers())
            .ThenBy(x => x.OnLap.PadNumbers())
            .ThenBy(x => x.Corner.PadNumbers());
    }
}
