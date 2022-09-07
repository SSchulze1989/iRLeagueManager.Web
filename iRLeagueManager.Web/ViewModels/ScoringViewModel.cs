using iRLeagueApiCore.Communication.Enums;
using iRLeagueApiCore.Communication.Models;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
            selectedSessions = new ObservableCollection<SessionModel>();
            this.model = model;
        }

        public long Id => model.Id;
        public long LeagueId => model.LeagueId;
        public long SeasonId => model.SeasonId;
        [Required]
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
        public bool TakeResultsFromExtSource
        {
            get => model.TakeResultsFromExtSource;
            set => SetP(model.TakeResultsFromExtSource, value => model.TakeResultsFromExtSource = value, value);
        }
        public bool UseResultSetTeam
        {
            get => model.UseResultSetTeam;
            set => SetP(model.UseResultSetTeam, value => model.UseResultSetTeam = value, value);
        }
        public bool UpdateTeamOnRecalculation
        {
            get => model.UpdateTeamOnRecalculation;
            set => SetP(model.UpdateTeamOnRecalculation, value => model.UpdateTeamOnRecalculation = value, value);
        }
        public int MaxResultsPerGroup
        {
            get => model.MaxResultsPerGroup;
            set => SetP(model.MaxResultsPerGroup, value => model.MaxResultsPerGroup = value, value);
        }
        public bool TakeGroupAverage
        {
            get => model.TakeGroupAverage;
            set => SetP(model.TakeGroupAverage, value => model.TakeGroupAverage = value, value);
        }
        public SessionType ScoringSessionType
        {
            get => model.ScoringSessionType;
            set => SetP(model.ScoringSessionType, value => model.ScoringSessionType = value, value);
        }
        public ScoringSessionSelectionType SessionSelectType
        {
            get => model.SessionSelectType;
            set => SetP(model.SessionSelectType, value => model.SessionSelectType = value, value);
        }
        public IEnumerable<long> SessionIds => model.SessionIds;

        private ObservableCollection<SessionModel> selectedSessions;
        public ObservableCollection<SessionModel> SelectedSessions
        {
            get => selectedSessions;
            set => Set(ref selectedSessions, value);
        }

        public void SetModel(ScoringModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }

        public async Task<bool> SaveCurrentModelAsync()
        {
            try
            {
                Loading = Saving = true;
                if (ApiService.CurrentLeague == null || model.Id == 0)
                {
                    return false;
                }
                await Task.Delay(500);
                Logger.LogInformation("Begin saving Scoring {ScoringId} ...", model.Id);
                var result = await ApiService.CurrentLeague
                    .Scorings()
                    .WithId(model.Id)
                    .Put(model);
                Logger.LogInformation("Result: {Status}|{StatusCode} - {ResultMessage}", result.Status, result.HttpStatusCode, result.Message);
                if (result.Success)
                {
                    HasChanged = false;
                }
                return result.Success;
            }
            finally
            {
                Loading = Saving = false;
            }
        }
    }
}