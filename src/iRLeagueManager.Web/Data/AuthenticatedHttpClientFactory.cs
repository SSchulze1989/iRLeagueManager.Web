namespace iRLeagueManager.Web.Server.Data;

public class AuthenticatedHttpClientFactory : IHttpClientFactory
{


    public HttpClient CreateClient(string name)
    {
        return new HttpClient();

    }
}
