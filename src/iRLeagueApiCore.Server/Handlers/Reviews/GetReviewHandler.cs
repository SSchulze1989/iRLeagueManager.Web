using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetReviewRequest(long ReviewId, bool IncludeComments) : IRequest<ReviewModel>;

public sealed class GetReviewHandler : ReviewsHandlerBase<GetReviewHandler, GetReviewRequest>, IRequestHandler<GetReviewRequest, ReviewModel>
{
    public GetReviewHandler(ILogger<GetReviewHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetReviewRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ReviewModel> Handle(GetReviewRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getReview = await MapToReviewModel(request.ReviewId, request.IncludeComments, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getReview;
    }
}
