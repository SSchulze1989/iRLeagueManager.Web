using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultConfigViewModel : LeagueViewModelBase<ResultConfigViewModel, ResultConfigModel>
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
        availableResultConfigs = new();
    }

    public long LeagueId => model.LeagueId;
    public long ResultConfigId => model.ResultConfigId;
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string DisplayName { get => model.DisplayName; set => SetP(model.DisplayName, value => model.DisplayName = value, value); }
    public ResultKind ResultKind { get => model.ResultKind; set => SetP(model.ResultKind, value => model.ResultKind = value, value); }
    public int ResultsPerTeam { get => model.ResultsPerTeam; set => SetP(model.ResultsPerTeam, value => model.ResultsPerTeam = value, value); }
    public ResultConfigInfoModel? SourceResultConfig { get => model.SourceResultConfig; set => SetP(model.SourceResultConfig, value => model.SourceResultConfig = value, value); }
    public long SourceResultConfigId
    {
        get => SourceResultConfig?.ResultConfigId ?? 0;
        set => SetP(SourceResultConfig, value => SourceResultConfig = value, GetConfigInfoModel(AvailableResultConfigs.FirstOrDefault(x => x.ResultConfigId == value)));
    }
    public bool CalculateCombinedResult
    {
        get => Scorings.Any(x => x.IsCombinedResult);
        set
        {
            if (value && CalculateCombinedResult == false)
            {
                var combined = AddScoring();
                combined.Name = "Combined";
                combined.ShowResults = true;
                combined.IsCombinedResult = true;
                return;
            }
            if (value == false && CalculateCombinedResult)
            {
                var removeScoring = Scorings.FirstOrDefault(x => x.IsCombinedResult);
                if (removeScoring != null)
                {
                    RemoveScoring(removeScoring);
                }
            }
        }
    }

    //private StandingConfigurationViewModel? standingConfig;
    //public StandingConfigurationViewModel? StandingConfig { get => standingConfig; set => Set(ref standingConfig, value); }
    //public bool CalculateStandings
    //{
    //    get => StandingConfig is not null;
    //    set
    //    {
    //        if (value && model.StandingConfig is null)
    //        {
    //            model.StandingConfig = new StandingConfigModel();
    //            StandingConfig = new(LoggerFactory, ApiService, model.StandingConfig);
    //        }
    //        if (value == false && model.StandingConfig is not null)
    //        {
    //            model.StandingConfig = null;
    //            StandingConfig = null;
    //        }
    //    }
    //}

    private ObservableCollection<ScoringViewModel> scorings;
    public ObservableCollection<ScoringViewModel> Scorings { get => scorings; set => SetP(scorings, value => scorings = value, value); }

    private ObservableCollection<ResultFilterViewModel> filtersForPoints;
    public ObservableCollection<ResultFilterViewModel> FiltersForPoints { get => filtersForPoints; set => Set(ref filtersForPoints, value); }

    private ObservableCollection<ResultFilterViewModel> filtersForResult;
    public ObservableCollection<ResultFilterViewModel> FiltersForResult { get => filtersForResult; set => Set(ref filtersForResult, value); }

    private ObservableCollection<ResultConfigModel> availableResultConfigs;
    public ObservableCollection<ResultConfigModel> AvailableResultConfigs { get => availableResultConfigs; set => Set(ref availableResultConfigs, value); }

    public override void SetModel(ResultConfigModel model)
    {
        base.SetModel(model);
        Scorings = new(model.Scorings.Select(scoringModel => new ScoringViewModel(LoggerFactory, ApiService, scoringModel)));
        FiltersForPoints = new(model.FiltersForPoints.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
        FiltersForResult = new(model.FiltersForResult.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
        //StandingConfig = model.StandingConfig is not null ? new StandingConfigurationViewModel(LoggerFactory, ApiService, model.StandingConfig) : null;
    }

    public async Task<StatusResult> LoadAvailableResultConfigs(CancellationToken cancellationToken)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        var request = ApiService.CurrentLeague.ResultConfigs()
            .Get(cancellationToken);
        var result = await request;
        if (result.Success && result.Content is IEnumerable<ResultConfigModel> configs)
        {
            AvailableResultConfigs = new(configs);
        }

        return result.ToStatusResult();
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

    public ScoringViewModel AddScoring()
    {
        var scoring = new ScoringModel() { Name = "New Scoring" };
        model.Scorings.Add(scoring);
        var newScoring = new ScoringViewModel(LoggerFactory, ApiService, scoring);
        Scorings.Add(newScoring);
        UpdateScoringIndex();
        return newScoring;
    }

    public void RemoveScoring(ScoringViewModel scoring)
    {
        Scorings.Remove(scoring);
        model.Scorings.Remove(scoring.GetModel());
        UpdateScoringIndex();
    }

    private void UpdateScoringIndex()
    {
        foreach (var (scoring, index) in model.Scorings.Where(x => x.IsCombinedResult == false).Select((x, i) => (x, i)))
        {
            scoring.Index = index;
        }
        var combinedScoring = model.Scorings.FirstOrDefault(x => x.IsCombinedResult);
        if (combinedScoring != null)
        {
            combinedScoring.Index = 999;
        }
    }

    public ResultConfigInfoModel? GetConfigInfoModel(ResultConfigModel? model)
    {
        if (model == null)
        {
            return null;
        }

        return new()
        {
            Name = model.Name,
            DisplayName = model.DisplayName,
            LeagueId = model.LeagueId,
            ResultConfigId = model.ResultConfigId,
        };
    }
}
