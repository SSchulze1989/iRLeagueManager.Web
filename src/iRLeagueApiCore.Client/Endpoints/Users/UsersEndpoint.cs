using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

internal sealed class UsersEndpoint : GetAllEndpoint<LeagueUserModel>, IUsersEndpoint, ILeagueUsersEndpoint
{
    public UsersEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Users");
    }

    public IUserByIdEndpoint WithId(string id)
    {
        return new UserByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }

    IPostEndpoint<object, string> IUsersEndpoint.ResendConfirmation()
    {
        return new ResendConfirmationEmailEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IPostEndpoint<IEnumerable<UserModel>, SearchModel> IUsersEndpoint.Search()
    {
        return new SearchEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ILeagueUserByIdEndpoint ILeagueUsersEndpoint.WithId(string id)
    {
        return new UserByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
