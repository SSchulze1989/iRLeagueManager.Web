namespace iRLeagueManager.Web.Server.Data;

internal sealed class AuthenticatedHttpClientFactory : IHttpClientFactory
{


    public HttpClient CreateClient(string name)
    {
        return new HttpClient();

    }
}
