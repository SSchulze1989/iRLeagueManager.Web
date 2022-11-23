using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultConfigViewModel : LeagueViewModelBase<ResultConfigViewModel, ResultConfigModel>
    {
        public ResultConfigViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
            : this(loggerFactory, apiService, new())
        {
        }

        public ResultConfigViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultConfigModel model)
            : base(loggerFactory, apiService, model)
        {
            scorings = new();
            filtersForPoints = new();
            filtersForResult = new();
        }

        public long LeagueId => model.LeagueId;
        public long ResultConfigId => model.ResultConfigId;
        public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
        public string DisplayName { get => model.DisplayName; set => SetP(model.DisplayName, value => model.DisplayName = value, value); }
        public ResultKind ResultKind { get => model.ResultKind; set => SetP(model.ResultKind, value => model.ResultKind = value, value); }

        private ObservableCollection<ScoringViewModel> scorings;
        public ObservableCollection<ScoringViewModel> Scorings { get => scorings; set => SetP(scorings, value => scorings = value, value); }

        private ObservableCollection<ResultFilterViewModel> filtersForPoints;
        public ObservableCollection<ResultFilterViewModel> FiltersForPoints { get => filtersForPoints; set => Set(ref filtersForPoints, value); }

        private ObservableCollection<ResultFilterViewModel> filtersForResult;
        public ObservableCollection<ResultFilterViewModel> FiltersForResult { get => filtersForResult; set => Set(ref filtersForResult, value); }

        public override void SetModel(ResultConfigModel model)
        {
            base.SetModel(model);
            Scorings = new(model.Scorings.Select(scoringModel => new ScoringViewModel(LoggerFactory, ApiService, scoringModel)));
            FiltersForPoints = new(model.FiltersForPoints.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
            FiltersForResult = new(model.FiltersForResult.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
        }

        public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (ApiService.CurrentLeague is null)
            {
                return LeagueNullResult();
            }

            try
            {
                Loading = true;
                var request = ApiService.CurrentLeague.ResultConfigs()
                .WithId(ResultConfigId)
                .Put(model, cancellationToken);
                var result = await request;

                if (result.Success && result.Content is not null)
                {
                    SetModel(result.Content);
                }

                return result.ToStatusResult();
            }
            finally
            {
                Loading = false;
            }
        }

        public void AddScoring()
        {
            var scoring = new ScoringModel() { Name = "New Scoring" };
            model.Scorings.Add(scoring);
            Scorings.Add(new ScoringViewModel(LoggerFactory, ApiService, scoring));
        }

        public void RemoveScoring(ScoringViewModel scoring)
        {
            Scorings.Remove(scoring);
            model.Scorings.Remove(scoring.GetModel());
        }
    }
}