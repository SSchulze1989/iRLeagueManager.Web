using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints.Users;
internal class ConfirmEmailEndpoint : PostEndpoint<object>
{
    public ConfirmEmailEndpoint(HttpClientWrapper httpClient, RouteBuilder routeBuilder, string token) : base(httpClient, routeBuilder)
    {
        RouteBuilder.AddEndpoint("confirm");
        RouteBuilder.AddParameter(token);
    }
}
