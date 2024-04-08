using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

internal sealed class ReviewByIdEndpoint : UpdateEndpoint<ReviewModel, PutReviewModel>, IReviewByIdEndpoint
{
    public ReviewByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long id) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }

    public IPostEndpoint<ReviewModel> MoveToSession(long sessionId)
    {
        return new MoveToSessionEndpoint(HttpClientWrapper, RouteBuilder, sessionId);
    }

    public IPostEndpoint<ReviewCommentModel, PostReviewCommentModel> ReviewComments()
    {
        return new ReviewCommentsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
