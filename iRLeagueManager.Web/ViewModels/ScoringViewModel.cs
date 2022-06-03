using iRLeagueApiCore.Communication.Enums;
using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class ScoringViewModel : LeagueViewModelBase<ScoringViewModel>
    {
        private ScoringModel model;

        public ScoringViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : this(loggerFactory, apiService, new ScoringModel())
        {
        }

        public ScoringViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ScoringModel model) : base (loggerFactory, apiService)
        {
            this.model = model;
        }

        public long Id => model.Id;
        public long LeagueId => model.LeagueId;
        public long SeasonId => model.SeasonId;
        public string Name
        {
            get => model.Name;
            set => SetP(model.Name, value => model.Name = value, value);
        }
        public ScoringKind ScoringKind
        {
            get => model.ScoringKind;
            set => SetP(model.ScoringKind, value => model.ScoringKind = value, value);
        }
        public string Description
        {
            get => model.Description;
            set => SetP(model.Description, value => model.Description = value, value);
        }
        public bool ShowResults
        {
            get => model.ShowResults;
            set => SetP(model.ShowResults, value => model.ShowResults = value, value);
        }

        public void SetModel(ScoringModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }

        public async Task<bool> SaveCurrentModelAsync()
        {
            if (ApiService.CurrentLeague == null || model.Id == 0)
            {
                return false;
            }
            Logger.LogInformation("Begin saving Scoring {ScoringId} ...", model.Id);
            var result = await ApiService.CurrentLeague
                .Scorings()
                .WithId(model.Id)
                .Put(model);
            Logger.LogInformation("Result: {Status}|{StatusCode} - {ResultMessage}", result.Status, result.HttpStatusCode, result.Message);
            return result.Success;
        }
    }
}