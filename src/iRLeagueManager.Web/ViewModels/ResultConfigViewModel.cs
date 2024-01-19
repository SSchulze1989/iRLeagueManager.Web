using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

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
        scorings ??= new();
        filtersForResult ??= new();
        availableResultConfigs ??= new();
        leagueMembers ??= new();
        teams ??= new();
    }

    public long LeagueId => model.LeagueId;
    public long ResultConfigId => model.ResultConfigId;
    public long? ChampSeasonId => model.ChampSeasonId;
    public string ChampionshipName => model.ChampionshipName;
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string DisplayName { get => model.DisplayName; set => SetP(model.DisplayName, value => model.DisplayName = value, value); }
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

    private ObservableCollection<ScoringViewModel> scorings;
    public ObservableCollection<ScoringViewModel> Scorings { get => scorings; set => Set(ref scorings, value); }

    public IEnumerable<ResultFilterModel> FiltersForPoints { get => model.FiltersForPoints; set => SetP(model.FiltersForPoints, value => model.FiltersForPoints = value.ToList(), value); }

    private ObservableCollection<ResultFilterViewModel> filtersForResult;
    public ObservableCollection<ResultFilterViewModel> FiltersForResult { get => filtersForResult; set => Set(ref filtersForResult, value); }

    private ObservableCollection<ResultConfigModel> availableResultConfigs;
    public ObservableCollection<ResultConfigModel> AvailableResultConfigs { get => availableResultConfigs; set => Set(ref availableResultConfigs, value); }

    private ObservableCollection<MemberModel> leagueMembers;
    public ObservableCollection<MemberModel> LeagueMembers { get => leagueMembers; set => Set(ref leagueMembers, value); }

    private ObservableCollection<TeamModel> teams;
    public ObservableCollection<TeamModel> Teams { get => teams; set => Set(ref teams, value); }

    public override void SetModel(ResultConfigModel model)
    {
        base.SetModel(model);
        Scorings = new(model.Scorings.Select(NewScoringViewModel));
        FiltersForResult = new(model.FiltersForResult.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
        ResetChangedState();
    }

    private ScoringViewModel NewScoringViewModel(ScoringModel model)
    {
        var scoringViewModel = new ScoringViewModel(LoggerFactory, ApiService, model);
        scoringViewModel.ParentViewModel = this;
        return scoringViewModel;
    }

    public async Task<StatusResult> Load(long resultConfigId, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague
                .ResultConfigs()
                .WithId(resultConfigId)
                .Get(cancellationToken);
            if (result.Success == false || result.Content is null)
            {
                return result.ToStatusResult();
            }
            SetModel(result.Content);
            return await LoadAvailableResultConfigs(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> LoadAvailableResultConfigs(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }
        if (CurrentSeason is null)
        {
            return SeasonNullResult();
        }

        var request = CurrentSeason.ResultsConfigs()
            .Get(cancellationToken);
        var result = await request;
        if (result.Success && result.Content is not null)
        {
            AvailableResultConfigs = new(result.Content);
        }

        return result.ToStatusResult();
    }

    public async Task<StatusResult> LoadLeagueMembersAndTeams(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var membersResult = await CurrentLeague.Members()
                .Get(cancellationToken);
            if (membersResult.Success == false || membersResult.Content is null)
            {
                return membersResult.ToStatusResult();
            }
            LeagueMembers = new(membersResult.Content);
            var teamsResult = await CurrentLeague.Teams()
                .Get(cancellationToken);
            if (teamsResult.Success == false || teamsResult.Content is null)
            {
                return teamsResult.ToStatusResult();
            }
            Teams = new(teamsResult.Content);
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
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
        var newScoring = NewScoringViewModel(scoring);
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
