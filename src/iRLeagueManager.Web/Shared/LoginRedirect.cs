using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;

namespace iRLeagueManager.Web.Shared
{
    public class LoginRedirect : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == false)
            {
                return;
            }
            var loadState = await AuthStateTask;

            if (loadState?.User.Identity?.IsAuthenticated == true)
            {
                // redirect to original page
                NavigationManager.NavigateTo(NavigationManager.Uri);
            }

            // redirect to login page
            var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
            NavigationManager.NavigateTo($"member/login?returnUrl={returnUrl}");
        }
    }
}
