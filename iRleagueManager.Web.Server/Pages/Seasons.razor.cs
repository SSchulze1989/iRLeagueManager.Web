using iRLeagueApiCore.Communication.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace iRleagueManager.Web.Server.Pages
{
    public partial class Seasons
    {
        [Parameter]
        public string LeagueName { get; set; }

        private string BaseUri = "https://irleaguemanager.net/irleagueapi";
        private SeasonModel[] SeasonList;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var token = await localStorage.GetItemAsStringAsync("ApiToken");

                if (string.IsNullOrEmpty(token))
                {
                    token = await Authenticator.AuthenticateAsync();
                    await localStorage.SetItemAsStringAsync("ApiToken", token);
                }

                if (string.IsNullOrEmpty(token) == false)
                {
                    Http.DefaultRequestHeaders.Add(
                        "Authorization",
                        "Bearer " + token
                    );
                }

                await FetchSeasons();
                StateHasChanged();
            }
        }

        private async Task FetchSeasons()
        {
            SeasonList = await Http.GetFromJsonAsync<SeasonModel[]>(BaseUri + $"/{LeagueName}/Season");
            var requestScheduleIds = SeasonList
                .SelectMany(x => x.ScheduleIds)
                .Select(x => $"ids={x}");
            var schedulesRequest = BaseUri + $"/{LeagueName}/Schedule?" + string.Join('&', requestScheduleIds);
            var schedules = await Http.GetFromJsonAsync<ScheduleModel[]>(schedulesRequest);

            var requestSessionIds = schedules
                .SelectMany(x => x.SessionIds)
                .Select(x => $"ids={x}");
            var sessionRequest = BaseUri + $"/{LeagueName}/Session?" + string.Join('&', requestSessionIds);
            var sessions = await Http.GetFromJsonAsync<GetSessionModel[]>(sessionRequest);

            foreach(var schedule in schedules)
            {
                schedule.Sessions = sessions.Where(x => x.ScheduleId == schedule.ScheduleId);
            }

            foreach(var season in SeasonList)
            {
                season.Schedules = schedules.Where(x => x.SeasonId == season.SeasonId);
            }
        }
    }

    public class SeasonModel : GetSeasonModel
    {
        public IEnumerable<ScheduleModel> Schedules { get; set; }
    }

    public class ScheduleModel : GetScheduleModel
    {
        public IEnumerable<GetSessionModel> Sessions { get; set; }
    }
}
