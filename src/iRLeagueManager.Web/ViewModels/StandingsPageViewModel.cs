using iRLeagueApiCore.Common.Models.Standings;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class StandingsPageViewModel : LeagueViewModelBase<StandingsPageViewModel>
    {
        public StandingsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
            base(loggerFactory, apiService)
        {
            standings = new ObservableCollection<StandingsModel>();
        }

        private ObservableCollection<StandingsModel> standings;
        public ObservableCollection<StandingsModel> Standings { get => standings; set => Set(ref standings, value); }

        private int selectedStandingIndex;
        public int SelectedStandingIndex { get => selectedStandingIndex; set { if (Set(ref selectedStandingIndex, value)) OnPropertyChanged(nameof(SelectedStanding)); } }

        public StandingsModel? SelectedStanding => Standings.ElementAtOrDefault(SelectedStandingIndex);

        public async Task<StatusResult> LoadFromEventAsync(long eventId)
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            var request = ApiService.CurrentLeague
                .Events()
                .WithId(eventId)
                .Standings()
                .Get();
            var result = await request;
            if (result.Success)
            {
                var standingsData = result.Content;
                Standings = new ObservableCollection<StandingsModel>(standingsData);
            }

            return result.ToStatusResult();
        }

        public async Task<StatusResult> LoadAsync(long? seasonId = null)
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            if (ApiService.CurrentSeason == null || ApiService.CurrentSeason.Id != seasonId && seasonId != null)
            {
                await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, seasonId.Value);
            }
            if (ApiService.CurrentSeason == null)
            {
                return SeasonNullResult();
            }

            var request = ApiService.CurrentSeason.Standings().Get();
            var result = await request;
            if (result.Success)
            {
                var standingsData = result.Content;
                Standings = new ObservableCollection<StandingsModel>(standingsData);
            }

            return result.ToStatusResult();
        }
    }
}
