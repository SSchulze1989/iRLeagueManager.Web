using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace iRLeagueManager.Web.Shared
{
    public class LeagueComponentBase : ComponentBase, IDisposable
    {
        public LeagueComponentBase()
        {
            Shared = new SharedStateService();
        }

        [Inject]
        public SharedStateService Shared { get; set; }

        private void StateStateChanged(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            Shared.StateChanged += StateStateChanged;
        }

        public void Dispose()
        {
            Shared.StateChanged -= StateStateChanged;
        }
    }
}
