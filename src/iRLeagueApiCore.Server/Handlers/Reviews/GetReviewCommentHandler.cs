using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetReviewCommentRequest(long CommentId) : IRequest<ReviewCommentModel>;

public sealed class GetReviewCommentHandler : CommentHandlerBase<GetReviewCommentHandler, GetReviewCommentRequest>,
    IRequestHandler<GetReviewCommentRequest, ReviewCommentModel>
{
    public GetReviewCommentHandler(ILogger<GetReviewCommentHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetReviewCommentRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ReviewCommentModel> Handle(GetReviewCommentRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getComment = await MapToReviewCommentModelAsync(request.CommentId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getComment;
    }
}
