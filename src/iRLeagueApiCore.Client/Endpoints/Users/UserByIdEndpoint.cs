using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

internal sealed class UserByIdEndpoint : EndpointBase, IUserByIdEndpoint, ILeagueUserByIdEndpoint
{
    public UserByIdEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, string id) :
        base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter(id);
    }

    IPostEndpoint<LeagueUserModel, RoleModel> ILeagueUserByIdEndpoint.AddRole()
    {
        return new AddRoleEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IPostEndpoint<object> IUserByIdEndpoint.ConfirmEmail(string token)
    {
        return new ConfirmEmailEndpoint(HttpClientWrapper, RouteBuilder, token);
    }

    async Task<ClientActionResult<UserModel>> IGetEndpoint<UserModel>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<UserModel>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<LeagueUserModel>> IGetEndpoint<LeagueUserModel>.Get(CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.GetAsClientActionResult<LeagueUserModel>(QueryUrl, cancellationToken);
    }

    async Task<ClientActionResult<PrivateUserModel>> IPutEndpoint<PrivateUserModel, PutUserModel>.Put(PutUserModel model, CancellationToken cancellationToken)
    {
        return await HttpClientWrapper.PutAsClientActionResult<PrivateUserModel>(QueryUrl, model, cancellationToken);
    }

    IPostEndpoint<LeagueUserModel, RoleModel> ILeagueUserByIdEndpoint.RemoveRole()
    {
        return new RemoveRoleEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
