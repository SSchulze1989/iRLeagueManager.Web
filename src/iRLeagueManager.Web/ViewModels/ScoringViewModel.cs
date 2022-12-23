using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace iRLeagueManager.Web.ViewModels
{
    public class ScoringViewModel : LeagueViewModelBase<ScoringViewModel, ScoringModel>
    {
        public ScoringViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ScoringModel model) :
            base(loggerFactory, apiService, model)
        {
            pointRule ??= new(loggerFactory, apiService);
        }

        public long Id => model.Id;
        public long LeagueId => model.LeagueId;
        [Required]
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
        public bool ShowResults { get => model.ShowResults; set => SetP(model.ShowResults, value => model.ShowResults = value, value); }
        public bool IsCombinedResult { get => model.IsCombinedResult; set => SetP(model.IsCombinedResult, value => model.IsCombinedResult = value, value); }
        public bool UseResultSetTeam { get => model.UseResultSetTeam; set => SetP(model.UseResultSetTeam, value => model.UseResultSetTeam = value, value); }
        public bool UpdateTeamOnRecalculation { get => model.UpdateTeamOnRecalculation; set => SetP(model.UpdateTeamOnRecalculation, value => model.UpdateTeamOnRecalculation = value, value); }
        public int MaxResultsPerGroup { get => model.MaxResultsPerGroup; set => SetP(model.MaxResultsPerGroup, value => model.MaxResultsPerGroup = value, value); }
        public bool CalcPoints { get => !model.UseSourcePoints; set => SetP(model.UseSourcePoints, value => model.UseSourcePoints = value, !value); }

        private PointRuleViewModel pointRule;
        public PointRuleViewModel PointRule { get => pointRule; set => Set(ref pointRule, value); }

        public override void SetModel(ScoringModel model)
        {
            this.model = model;
            model.PointRule ??= new();
            PointRule = new(LoggerFactory, ApiService, model.PointRule);
            OnPropertyChanged();
        }
    }
}