using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.Reviews;

internal sealed class ReviewsEndpoint : PostGetAllEndpoint<ReviewModel, PostReviewModel>, IReviewsEndpoint, IGetAllEndpoint<ReviewModel>, IPostEndpoint<ReviewModel, PostReviewModel>
{
    public ReviewsEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Reviews");
    }

    public IReviewByIdEndpoint WithId(long id)
    {
        return new ReviewByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
