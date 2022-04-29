using System.Net.Http;

namespace iRleagueManager.Web.Server.Data
{
    public class AuthenticatedHttpClientFactory : IHttpClientFactory
    {


        public HttpClient CreateClient(string name)
        {
            return new HttpClient();

        }
    }
}
