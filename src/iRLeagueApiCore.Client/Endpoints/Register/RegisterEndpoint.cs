using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Register;
internal sealed class RegisterEndpoint : PostEndpoint<UserModel, RegisterModel>
{
    public RegisterEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Register");
    }
}
