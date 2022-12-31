using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class TeamViewModel : LeagueViewModelBase<TeamViewModel, TeamModel>
{
    public TeamViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public TeamViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, TeamModel model) : 
        base(loggerFactory, apiService, model)
    {
    }

    public long TeamId => model.TeamId;
    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }
    public string Profile { get => model.Profile; set => SetP(model.Profile, value => model.Profile = value, value); }
    public string TeamColor { get => model.TeamColor; set => SetP(model.TeamColor, value => model.TeamColor = value, value); }
    public string TeamHomepage { get => model.TeamHomepage; set => SetP(model.TeamHomepage, value => model.TeamHomepage = value, value); }
    public IList<MemberInfoModel> Members { get => (IList<MemberInfoModel>)model.Members; set => SetP(model.Members, value => model.Members = value, value); }

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
                .Teams()
                .WithId(TeamId)
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
