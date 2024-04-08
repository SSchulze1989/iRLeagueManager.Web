using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

public interface IReviewCommentByIdEndpoint : IUpdateEndpoint<ReviewCommentModel, PutReviewCommentModel>
{
}
