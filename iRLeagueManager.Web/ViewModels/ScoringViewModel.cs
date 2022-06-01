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
    }
}