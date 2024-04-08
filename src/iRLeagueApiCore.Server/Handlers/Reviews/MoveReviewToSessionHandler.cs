using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record MoveReviewToSessionRequest(long SessionId, long ReviewId, LeagueUser User) : IRequest<ReviewModel>;
public sealed class MoveReviewToSessionHandler : ReviewsHandlerBase<MoveReviewToSessionHandler, MoveReviewToSessionRequest>,
    IRequestHandler<MoveReviewToSessionRequest, ReviewModel>
{
    /// <summary>
    /// Always include comments because this can only be called by an authorized user
    /// </summary>
    private const bool includeComments = true;

    public MoveReviewToSessionHandler(ILogger<MoveReviewToSessionHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<MoveReviewToSessionRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ReviewModel> Handle(MoveReviewToSessionRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var moveReview = await GetReviewEntity(request.ReviewId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        var toSession = await GetSessionEntityAsync(request.SessionId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        moveReview = await MoveToSessionAsync(request.User, moveReview, toSession, cancellationToken);
        await dbContext.SaveChangesAsync();
        var getReview = await MapToReviewModel(moveReview.ReviewId, includeComments, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getReview;
    }

    private async Task<IncidentReviewEntity> MoveToSessionAsync(LeagueUser user, IncidentReviewEntity review, SessionEntity session, CancellationToken cancellationToken)
    {
        review.Session = session;
        return await Task.FromResult(UpdateVersionEntity(user, review));
    }

    private async Task<SessionEntity?> GetSessionEntityAsync(long sessionId, CancellationToken cancellationToken)
    {
        return await dbContext.Sessions
            .Where(x => x.SessionId == sessionId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
