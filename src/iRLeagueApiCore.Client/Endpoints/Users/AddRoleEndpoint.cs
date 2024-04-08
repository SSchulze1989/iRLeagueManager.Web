using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

internal sealed class AddRoleEndpoint : PostEndpoint<LeagueUserModel, RoleModel>, IPostEndpoint<LeagueUserModel, RoleModel>
{
    public AddRoleEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("AddRole");
    }
}
