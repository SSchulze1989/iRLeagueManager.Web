using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;

namespace iRLeagueApiCore.Client.Endpoints.Users;
internal sealed class ResendConfirmationEmailEndpoint : PostEndpoint<object, string>
{
    public ResendConfirmationEmailEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("ResendConfirmation");
    }
}
