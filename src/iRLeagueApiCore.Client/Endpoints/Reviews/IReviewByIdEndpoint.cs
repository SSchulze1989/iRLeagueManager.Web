using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

public interface IReviewByIdEndpoint : IUpdateEndpoint<ReviewModel, PutReviewModel>
{
    public IPostEndpoint<ReviewCommentModel, PostReviewCommentModel> ReviewComments();
    public IPostEndpoint<ReviewModel> MoveToSession(long sessionId);
}
