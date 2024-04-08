using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

internal sealed class ReviewCommentsEndpoint : PostEndpoint<ReviewCommentModel, PostReviewCommentModel>, IReviewCommentsEndpoint
{
    public ReviewCommentsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("ReviewComments");
    }

    public IReviewCommentByIdEndpoint WithId(long id)
    {
        return new ReviewCommentByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
