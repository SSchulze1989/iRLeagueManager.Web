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
        set => SetP(SourceResultConfig, value => SourceResultConfig = value, AvailableResultConfigs.FirstOrDefault(x => x.ResultConfigId == value));
    }
    public bool CalculateCombinedResult
    {
        get => Scorings.Any(x => x.IsCombinedResult);
        set
        {
            if (value && CalculateCombinedResult == false)
            {
                var combined = AddScoring(combined: true);
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

    public IEnumerable<FilterConditionModel> FiltersForPoints
    {
        get => model.FiltersForPoints.Select(x => x.Condition);
        set => SetP(model.FiltersForPoints, value => model.FiltersForPoints = value, GetFilterConditions(model.FiltersForPoints, value));
    }

    private ObservableCollection<ResultFilterViewModel> filtersForResult;
    public ObservableCollection<ResultFilterViewModel> FiltersForResult { get => filtersForResult; set => Set(ref filtersForResult, value); }

    private ObservableCollection<ResultConfigInfoModel> availableResultConfigs;
    public ObservableCollection<ResultConfigInfoModel> AvailableResultConfigs { get => availableResultConfigs; set => Set(ref availableResultConfigs, value); }

    private ObservableCollection<MemberModel> leagueMembers;
    public ObservableCollection<MemberModel> LeagueMembers { get => leagueMembers; set => Set(ref leagueMembers, value); }

    private ObservableCollection<TeamModel> teams;
    public ObservableCollection<TeamModel> Teams { get => teams; set => Set(ref teams, value); }

    public int RaceCount { get => Scorings.Where(x => !x.IsCombinedResult).Count(); set => SetRaceCount(value); }

    protected override void SetModel(ResultConfigModel model)
    {
        base.SetModel(model);
        Scorings = new(model.Scorings.Select(NewScoringViewModel));
        FiltersForResult = new(model.FiltersForResult.Select(filter => new ResultFilterViewModel(LoggerFactory, ApiService, filter)));
        ResetChangedState();
    }
    private static IList<ResultFilterModel> GetFilterConditions(IEnumerable<ResultFilterModel> filters, IEnumerable<FilterConditionModel> conditions)
    {
        var updatedFilters = filters.ToList();
        for (int i = 0; i < Math.Max(filters.Count(), conditions.Count()); i++)
        {
            var filter = filters.ElementAtOrDefault(i);
            var condition = conditions.ElementAtOrDefault(i);
            if (condition is null)
            {
                updatedFilters.Remove(filter!);
                continue;
            }
            if (filter is null)
            {
                filter = new();
                updatedFilters.Add(filter);
            }
            filter.Condition = condition;
        }
        return updatedFilters;
    }

    private ScoringViewModel NewScoringViewModel(ScoringModel model)
    {
        var scoringViewModel = new ScoringViewModel(LoggerFactory, ApiService, model)
        {
            ParentViewModel = this
        };
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
            AvailableResultConfigs = new(result.Content
                .Select(GetConfigInfoModel)
                .NotNull()
                .Where(x => x.ChampSeasonId != ChampSeasonId));
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

            UpdateModelScorings();
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

    public ScoringViewModel AddScoring(bool combined = false)
    {
        var scoring = model.Scorings
            .Where(x => x.IsCombinedResult == combined)
            .ElementAtOrDefault(combined ? 0 : RaceCount)
            ?? new ScoringModel() { Name = $"Race {RaceCount + 1}" };
        var scoringViewModel = NewScoringViewModel(scoring);
        Scorings.Insert(RaceCount, (scoringViewModel));
        UpdateScoringIndex();
        Changed();
        return scoringViewModel;
    }

    public void RemoveScoring(ScoringViewModel scoring)
    {
        Scorings.Remove(scoring);
        Changed();
        UpdateScoringIndex();
    }

    private void UpdateScoringIndex()
    {
        foreach (var (scoring, index) in Scorings.Where(x => x.IsCombinedResult == false).WithIndex())
        {
            scoring.Index = index;
        }
        var combinedScoring = Scorings.FirstOrDefault(x => x.IsCombinedResult);
        if (combinedScoring != null)
        {
            combinedScoring.Index = 999;
        }
    }

    private void UpdateModelScorings()
    {
        model.Scorings = Scorings.Select(x => x.GetModel()).ToList();
    }

    public ResultConfigInfoModel? GetConfigInfoModel(ResultConfigModel? model)
    {
        if (model == null)
        {
            return null;
        }

        return new()
        {
            ChampionshipName = model.ChampionshipName,
            ChampSeasonId = model.ChampSeasonId,
            Name = model.Name,
            DisplayName = model.DisplayName,
            LeagueId = model.LeagueId,
            ResultConfigId = model.ResultConfigId,
        };
    }

    private void SetRaceCount(int count)
    {
        while (count < RaceCount && RaceCount > 0)
        {
            RemoveScoring(Scorings.Last());
        }
        while (count > RaceCount)
        {
            AddScoring();
        }
    }
}
