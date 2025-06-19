using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.Pages;

namespace iRLeagueManager.Web.ViewModels;

public class MemberServiceViewModel : LeagueViewModelBase
{
    public MemberServiceViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : base(loggerFactory, apiService)
    {
    }

    public async Task<StatusResult<IEnumerable<MemberModel>>> GetLeagueMembers(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult(Enumerable.Empty<MemberModel>());
        }

        try
        {
            Loading = true;
            var membersResult = await CurrentLeague.Members()
                .Get(cancellationToken).ConfigureAwait(false);
            return membersResult.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult<IEnumerable<TeamModel>>> GetLeagueTeams(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult(Enumerable.Empty<TeamModel>());
        }

        try
        {
            Loading = true;
            var teamsResult = await CurrentLeague.Teams()
                .Get(cancellationToken).ConfigureAwait(false);
            if (teamsResult.Success == false || teamsResult.Content is null)
            {
                return teamsResult.ToContentStatusResult(Enumerable.Empty<TeamModel>());
            }
            return teamsResult.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult<IEnumerable<RosterInfoModel>>> GetLeagueRosters(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult(Enumerable.Empty<RosterInfoModel>());
        }
        try
        {
            Loading = true;
            var rostersResult = await CurrentLeague.Rosters()
                .Get(cancellationToken).ConfigureAwait(false);
            if (rostersResult.Success == false || rostersResult.Content is null)
            {
                return rostersResult.ToContentStatusResult([]);
            }
            return rostersResult.ToContentStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}