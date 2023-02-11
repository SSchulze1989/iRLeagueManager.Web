using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ChampionshipViewModel : LeagueViewModelBase<ChampionshipViewModel, ChampionshipModel>
{
    public ChampionshipViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    { }

    public ChampionshipViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ChampionshipModel model) : 
        base(loggerFactory, apiService, model)
    { }

    public long ChampionshipId => model.ChampionshipId;
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string DisplayName { get => model.DisplayName; set => SetP(model.DisplayName, value => model.DisplayName = value, value); }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Championships()
                .WithId(ChampionshipId)
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
}