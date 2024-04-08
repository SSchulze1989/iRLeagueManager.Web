using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

internal sealed class SearchEndpoint : PostEndpoint<IEnumerable<UserModel>, SearchModel>, IPostEndpoint<IEnumerable<UserModel>, SearchModel>
{
    public SearchEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Search");
    }
}
