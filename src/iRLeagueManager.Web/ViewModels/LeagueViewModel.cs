using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class LeagueViewModel : LeagueViewModelBase<LeagueViewModel, LeagueModel>
{
    public LeagueViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public LeagueViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, LeagueModel model) :
        base(loggerFactory, apiService, model)
    {
        seasons = new ObservableCollection<SeasonViewModel>();
    }

    public long LeagueId => model.Id;
    public string LeagueName { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }

    public string NameFull { get => model.Name; set => SetP(model.NameFull, value => model.Name = value, value); }

    private ObservableCollection<SeasonViewModel> seasons;
    public ObservableCollection<SeasonViewModel> Seasons { get => seasons; set => Set(ref seasons, value); }

    public bool EnableProtests { get => model.EnableProtests; set => SetP(model.EnableProtests, value => model.EnableProtests = value, value); }
    public TimeSpan ProtestCoolDownPeriod { get => model.ProtestCoolDownPeriod; set => SetP(model.ProtestCoolDownPeriod, value => model.ProtestCoolDownPeriod = value, value); }
    public TimeSpan ProtestsClosedAfter { get => model.ProtestsClosedAfter; set => SetP(model.ProtestsClosedAfter, value => model.ProtestsClosedAfter = value, value); }
    public ProtestPublicSetting ProtestPublic { get => model.ProtestsPublic; set => SetP(model.ProtestsPublic, value => model.ProtestsPublic = value, value); }

    public async Task<StatusResult> LoadCurrent(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.Client.Leagues()
                .WithName(ApiService.CurrentLeague.Name)
                .Get(cancellationToken);
            var result = await request;
            if (result.Success && result.Content is LeagueModel leagueModel)
            {
                SetModel(leagueModel);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> LoadSeasons(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Seasons()
                .Get(cancellationToken);
            var result = await request;
            if (result.Success && result.Content is IEnumerable<SeasonModel> seasonModels)
            {
                Seasons = new(seasonModels.Select(x => new SeasonViewModel(LoggerFactory, ApiService, x)));
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddSeason(SeasonModel season, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Seasons()
                .Post(season, cancellationToken);
            var result = await request;
            if (result.Success == false || result.Content is not SeasonModel newSeason)
            {
                return result.ToStatusResult();
            }
            var scheduleRequest = ApiService.CurrentLeague.Seasons()
                .WithId(newSeason.SeasonId)
                .Schedules()
                .Post(new() { Name = "Schedule" }, cancellationToken);
            await scheduleRequest;
            return await LoadSeasons(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteSeason(SeasonModel season, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Seasons()
                .WithId(season.SeasonId)
                .Delete(cancellationToken);
            var result = await request;
            if (result.Success)
            {
                return await LoadSeasons(cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}