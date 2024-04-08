using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Reviews;

namespace iRLeagueApiCore.Client.Endpoints.VoteCategories;

internal sealed class VoteCategoryByIdEndpoint : UpdateEndpoint<VoteCategoryModel, PutVoteCategoryModel>, IVoteCategoryByIdEndpoint
{
    public VoteCategoryByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long id) : 
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }
}