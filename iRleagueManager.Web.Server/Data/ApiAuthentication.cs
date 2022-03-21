using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace iRleagueManager.Web.Server.Data
{
    public class ApiAuthentication
    {
        private string AuthRequestUrl = "https://irleaguemanager.net/irleagueapi/Authenticate/login";
        private HttpClient Http { get; }
        public bool IsLoggedIn { get; private set; }

        private string defUserName = "testuser";
        private string defPassword = "TestPass123!";

        public ApiAuthentication(HttpClient http)
        {
            Http = http;
        }
        
        public async Task<string> AuthenticateAsync()
        {
            var login = new LoginModel()
            {
                Username = defUserName,
                Password = defPassword
            };
            var request = await Http.PostAsJsonAsync(AuthRequestUrl, login);

            if (request.IsSuccessStatusCode == false)
            {
                return "";
            }

            var response = await request.Content.ReadFromJsonAsync<TokenResponse>();
            return response.Token;
        }

        private class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
            public DateTime Expires { get; set; }
        }
    }
}
