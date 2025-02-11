using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SeasonViewModel : LeagueViewModelBase<SeasonViewModel, SeasonModel>
{
    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new SeasonModel())
    {
    }

    public SeasonViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, SeasonModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long SeasonId => model.SeasonId;
    public long LeagueId => model.LeagueId;
    public DateTime? SeasonStart => model.SeasonStart;
    public DateTime? SeasonEnd => model.SeasonEnd;
    public string Name { get => model.SeasonName; set => SetP(model.SeasonName, value => model.SeasonName = value, value); }
    public long? MainScoringId { get => model.MainScoringId; set => SetP(model.MainScoringId, value => model.MainScoringId = value, value); }
    public bool HideComments { get => model.HideComments; set => SetP(model.HideComments, value => model.HideComments = value, value); }
    public bool Finished { get => model.Finished; set => SetP(model.Finished, value => model.Finished = value, value); }
    public IEnumerable<long> ScheduleIds => model.ScheduleIds;

    protected override void SetModel(SeasonModel model)
    {
        model ??= new SeasonModel();
        this.model = model;
        OnPropertyChanged();
    }

    public async Task Load(long seasonId)
    {
        try
        {
            Loading = true;
            if (ApiService.CurrentLeague == null)
            {
                return;
            }
            await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, seasonId);
            if (ApiService.CurrentSeason == null)
            {
                return;
            }
            var result = await ApiService.CurrentSeason.Get().ConfigureAwait(false);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
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
            var request = ApiService.CurrentLeague.Seasons()
                .WithId(SeasonId)
                .Put(model, cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success && result.Content is SeasonModel seasonModel)
            {
                SetModel(seasonModel);
            }

            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<bool> SaveCurrentModel()
    {
        try
        {
            Loading = true;
            await Task.Delay(1000);
            if (model.SeasonId == 0 || ApiService.CurrentLeague == null)
            {
                return false;
            }
            var result = await ApiService.CurrentLeague
                .Seasons()
                .WithId(model.SeasonId)
                .Put(model).ConfigureAwait(false);
            if (result.Success && result.Content is not null)
            {
                model = result.Content;
                return true;
            }
            return false;
        }
        finally
        {
            Loading = false;
        }
    }
}
