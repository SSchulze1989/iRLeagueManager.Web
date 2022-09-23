using iRLeagueManager.Web.Data;
using Microsoft.AspNetCore.Components;
using MvvmBlazor.Components;
using System.ComponentModel;

namespace iRLeagueManager.Web.Shared
{
    public abstract partial class LeagueComponentBase : MvvmComponentBase
    {
        public LeagueComponentBase()
        {
        }

        [Inject]
        public SharedStateService Shared { get; set; } = default!;
        [Inject]
        public LeagueApiService ApiService { get; set; } = default!;

        [Parameter]
        public string? LeagueName { get; set; }
        [Parameter]
        public long? SeasonId { get; set; }

        protected bool ParametersSet { get; set; } = false;
        protected bool HasRendered { get; set; } = false;

        protected virtual void SharedStateChanged(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        protected virtual void RedirectUrl()
        {
        }

        protected override void OnParametersSet()
        {
            if (SeasonId == null && Shared.SeasonId != 0)
            {
                SeasonId = Shared.SeasonId;
                RedirectUrl();
            }
            ParametersSet = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender == false || ParametersSet == false)
            {
                return;
            }

            if (LeagueName != null)
            {
                await ApiService.SetCurrentLeagueAsync(LeagueName);
                if (SeasonId != null)
                {
                    await ApiService.SetCurrentSeasonAsync(LeagueName, SeasonId.Value);
                }
            }
            
            HasRendered = true;
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            Shared.StateChanged += SharedStateChanged;
        }

        protected override void Dispose(bool disposing)
        {
            Shared.StateChanged -= SharedStateChanged;
            base.Dispose(disposing);
        }
    }
}
