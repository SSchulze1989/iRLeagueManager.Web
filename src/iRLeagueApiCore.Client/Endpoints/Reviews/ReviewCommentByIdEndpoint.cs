using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

internal sealed class ReviewCommentByIdEndpoint : UpdateEndpoint<ReviewCommentModel, PutReviewCommentModel>, IReviewCommentByIdEndpoint
{
    public ReviewCommentByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long id) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }
}
