using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RosterViewModel : LeagueViewModelBase<RosterViewModel, RosterModel>
{
    public RosterViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
        : base(loggerFactory, apiService, new())
    {
    }

    public RosterViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RosterModel model)
        : base(loggerFactory, apiService, model)
    {
    }

    public long RosterId { get => model.RosterId; }
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string Description { get => model.Description; set => SetP(model.Description, value => model.Description = value, value); }
    public IEnumerable<RosterMemberModel> RosterEntries { get => model.RosterEntries; set => SetP(model.RosterEntries, value => model.RosterEntries = value, value); }

    public async Task<StatusResult> Load(long rosterId, CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Rosters()
                .WithId(rosterId)
                .Get(cancellationToken)
                .ConfigureAwait(false);
            if (result.Success && result.Content is RosterModel model)
            {
                SetModel(model);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> Save(CancellationToken cancellationToken = default)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Rosters()
                .WithId(RosterId)
                .Put(this.model, cancellationToken);
            if (result.Success && result.Content is RosterModel model)
            {
                SetModel(model);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
