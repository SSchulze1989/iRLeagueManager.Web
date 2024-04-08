using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Extensions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetReviewsFromEventRequest(long EventId, bool IncludeComments) : IRequest<IEnumerable<ReviewModel>>;
public sealed class GetReviewsFromEventHandler : ReviewsHandlerBase<GetReviewsFromEventHandler, GetReviewsFromEventRequest>,
    IRequestHandler<GetReviewsFromEventRequest, IEnumerable<ReviewModel>>
{
    public GetReviewsFromEventHandler(ILogger<GetReviewsFromEventHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetReviewsFromEventRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ReviewModel>> Handle(GetReviewsFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getReviews = await MapToGetReviewsFromEventAsync(request.EventId, request.IncludeComments, cancellationToken);
        return getReviews.OrderBy(x => x.SessionNr).ThenBy(x => x.IncidentNr);
    }

    private async Task<IEnumerable<ReviewModel>> MapToGetReviewsFromEventAsync(long EventId, bool includeComments, CancellationToken cancellationToken)
    {
        return (await dbContext.IncidentReviews
            .Where(x => x.Session.EventId == EventId)
            .Select(MapToReviewModelExpression(includeComments))
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.SessionNr)
            .ThenBy(x => x.IncidentNr.PadNumbers())
            .ThenBy(x => x.OnLap.PadNumbers())
            .ThenBy(x => x.Corner.PadNumbers());
    }
}
