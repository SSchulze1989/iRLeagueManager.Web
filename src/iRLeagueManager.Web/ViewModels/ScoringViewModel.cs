using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
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
            this.model = model;
        }

        public long Id => model.Id;
        public long LeagueId => model.LeagueId;
        [Required]
        public string Name { get => model.Name; set => Set(model, x => x.Name, value); }
        public ScoringKind ScoringKind { get => model.ScoringKind; set => Set(model, x => x.ScoringKind, value); }
        public bool ShowResults { get => model.ShowResults; set => Set(model, x => x.ShowResults, value); }
        public bool UseResultSetTeam { get => model.UseResultSetTeam; set => Set(model, x => x.UseResultSetTeam, value); }
        public bool UpdateTeamOnRecalculation { get => model.UpdateTeamOnRecalculation; set => Set(model, x => x.UpdateTeamOnRecalculation, value); }
        public int MaxResultsPerGroup { get => model.MaxResultsPerGroup; set => Set(model, x => x.MaxResultsPerGroup, value); }

        public void SetModel(ScoringModel model)
        {
            this.model = model;
            OnPropertyChanged(null);
        }

        public async Task<bool> SaveCurrentModelAsync()
        {
            //try
            //{
            //    Loading = Saving = true;
            //    if (ApiService.CurrentLeague == null || model.Id == 0)
            //    {
            //        return false;
            //    }
            //    await Task.Delay(500);
            //    Logger.LogInformation("Begin saving Scoring {ScoringId} ...", model.Id);
            //    var result = await ApiService.CurrentLeague
            //        .Scorings()
            //        .WithId(model.Id)
            //        .Put(model);
            //    Logger.LogInformation("Result: {Status}|{StatusCode} - {ResultMessage}", result.Status, result.HttpStatusCode, result.Message);
            //    if (result.Success)
            //    {
            //        HasChanged = false;
            //    }
            //    return result.Success;
            //}
            //finally
            //{
            //    Loading = Saving = false;
            //}
            return await Task.FromResult(true);
        }
    }
}