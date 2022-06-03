using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ScoringsViewModel : LeagueViewModelBase<ScoringsViewModel>
    {
        public ScoringsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : base(loggerFactory, apiService)
        {
            scorings = new ObservableCollection<ScoringViewModel>();
            selected = new ScoringViewModel(loggerFactory, apiService);
        }

        private ObservableCollection<ScoringViewModel> scorings;
        public ObservableCollection<ScoringViewModel> Scorings { get => scorings; set => Set(ref scorings, value); }

        private ScoringViewModel selected;
        public ScoringViewModel Selected { get => selected; set => Set(ref selected, value); }

        public async Task LoadFromSeason(long seasonId)
        {
            if (ApiService.CurrentLeague == null)
            {
                return;
            }
            await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, seasonId);
            if (ApiService.CurrentSeason == null)
            {
                return;
            }
            var result = await ApiService.CurrentSeason.Scorings().Get();
            if (result.Success)
            {
                var scorings = result.Content.Select(x => new ScoringViewModel(LoggerFactory, ApiService, x));
                Scorings = new ObservableCollection<ScoringViewModel>(scorings);
            }
        }
    }
}
