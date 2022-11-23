using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
        public string Name { get => model.Name; set => Set(model, x => x.Name, value); }
        public bool ShowResults { get => model.ShowResults; set => Set(model, x => x.ShowResults, value); }
        public bool UseResultSetTeam { get => model.UseResultSetTeam; set => Set(model, x => x.UseResultSetTeam, value); }
        public bool UpdateTeamOnRecalculation { get => model.UpdateTeamOnRecalculation; set => Set(model, x => x.UpdateTeamOnRecalculation, value); }
        public int MaxResultsPerGroup { get => model.MaxResultsPerGroup; set => Set(model, x => x.MaxResultsPerGroup, value); }

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