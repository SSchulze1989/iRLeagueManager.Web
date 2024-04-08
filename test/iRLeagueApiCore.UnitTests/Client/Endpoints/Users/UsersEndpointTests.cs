using iRLeagueApiCore.Client.Endpoints.Users;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.UnitTests.Client.Endpoints.Users;

public sealed class UsersEndpointTests
{
    private readonly string usersController = "Users";
    private readonly string searchEndpoint = "Search";
    private readonly string addRoleEndpoint = "AddRole";
    private readonly string removeRoleEndpoint = "RemoveRole";
    private readonly string testLeague = "TestLeague";
    private readonly string testUserId = "1234-56789";
    private readonly PutUserModel testPutUser = new();
    private static RouteBuilder BaseRouteBuilder => new();
    private RouteBuilder LeagueRouteBuilder => (RouteBuilder)(new RouteBuilder().AddEndpoint(testLeague));

    [Fact]
    public async void ShouldCallRequestGetAllUsers()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{testLeague}/{usersController}";
        await EndpointsTests.TestRequest<ILeagueUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, LeagueRouteBuilder),
            x => x.Get(),
            HttpMethod.Get);
    }

    [Fact]
    public async void ShouldCallRequestGetLeagueUser()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{testLeague}/{usersController}/{testUserId}";
        await EndpointsTests.TestRequest<IUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, LeagueRouteBuilder),
            x => x.WithId(testUserId).Get(),
            HttpMethod.Get);
    }

    [Fact]
    public async void ShouldCallRequestGetUser()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{usersController}/{testUserId}";
        await EndpointsTests.TestRequest<IUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, BaseRouteBuilder),
            x => x.WithId(testUserId).Get(),
            HttpMethod.Get);
    }

    [Fact]
    public async void ShouldCallRequestPutUser()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{usersController}/{testUserId}";
        await EndpointsTests.TestRequest<IUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, BaseRouteBuilder),
            x => x.WithId(testUserId).Put(testPutUser),
            HttpMethod.Put);
    }

    [Fact]
    public async void ShouldCallRequestSearch()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{usersController}/{searchEndpoint}";
        await EndpointsTests.TestRequest<IUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, BaseRouteBuilder),
            x => x.Search().Post(new()),
            HttpMethod.Post);
    }

    [Fact]
    public async void ShouldCallRequestAddRole()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{testLeague}/{usersController}/{testUserId}/{addRoleEndpoint}";
        await EndpointsTests.TestRequest<ILeagueUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, LeagueRouteBuilder),
            x => x.WithId(testUserId).AddRole().Post(new()),
            HttpMethod.Post);
    }

    [Fact]
    public async void ShouldCallRequestRemoveRole()
    {
        string shouldRequestUrl = $"{EndpointsTests.BaseUrl}{testLeague}/{usersController}/{testUserId}/{removeRoleEndpoint}";
        await EndpointsTests.TestRequest<ILeagueUsersEndpoint>(shouldRequestUrl,
            x => new UsersEndpoint(x, LeagueRouteBuilder),
            x => x.WithId(testUserId).RemoveRole().Post(new()),
            HttpMethod.Post);
    }
}
