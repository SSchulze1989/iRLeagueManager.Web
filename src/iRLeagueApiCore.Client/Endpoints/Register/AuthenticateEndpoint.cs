using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Register;
internal sealed class AuthenticateEndpoint : EndpointBase, IAuthenticateEndpoint
{
    public AuthenticateEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder) : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddParameter("Authenticate");
    }

    public IPostEndpoint<UserModel, RegisterModel> Register()
    {
        return new RegisterEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
