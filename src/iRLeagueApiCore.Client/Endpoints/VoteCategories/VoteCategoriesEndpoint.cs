using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.VoteCategories;

internal sealed class VoteCategoriesEndpoint : PostGetAllEndpoint<VoteCategoryModel, PostVoteCategoryModel>, IVoteCategoriesEndpoint
{
    public VoteCategoriesEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("VoteCategories");
    }

    IVoteCategoryByIdEndpoint IWithIdEndpoint<IVoteCategoryByIdEndpoint, long>.WithId(long id)
    {
        return new VoteCategoryByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
