using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class TeamsViewModel : LeagueViewModelBase<TeamsViewModel>
{
    public TeamsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
        base(loggerFactory, apiService)
    {
        teams = new();
        members = new();
    }

    private ObservableCollection<TeamViewModel> teams;
    public ObservableCollection<TeamViewModel> Teams { get => teams; set => Set(ref teams, value); }

    private ObservableCollection<MemberInfoModel> members;
    public ObservableCollection<MemberInfoModel> Members { get => members; set => Set(ref members, value); }

    public async Task<StatusResult> LoadFromLeagueAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Teams()
                .Get(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success && result.Content is not null)
            {
                Teams = new(result.Content.Select(x => new TeamViewModel(LoggerFactory, ApiService, x)));
            }
            return await LoadMembersAsync(cancellationToken);
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> AddTeam(TeamModel team, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Teams()
                .Post(team, cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success)
            {
                return await LoadFromLeagueAsync(cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> DeleteTeam(TeamModel model, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Teams()
                .WithId(model.TeamId)
                .Delete(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success)
            {
                return await LoadFromLeagueAsync(cancellationToken);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> LoadMembersAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = false;
            var request = ApiService.CurrentLeague
                .Members()
                .Get(cancellationToken).ConfigureAwait(false);
            var result = await request;
            if (result.Success && result.Content is not null)
            {
                Members = new(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
