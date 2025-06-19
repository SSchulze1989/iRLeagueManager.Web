using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public class MemberViewModel : LeagueViewModelBase<MemberViewModel, MemberModel>
{
    public long MemberId { get => model.MemberId; }

    public string Firstname { get => model.Firstname; set => SetP(Firstname, value => model.Firstname = value, value); }

    public string Lastname { get => model.Lastname; set => SetP(Lastname, value => model.Lastname = value, value); }

    public string FullName { get => $"{model.Firstname} {model.Lastname}"; }

    public string IRacingId { get => model.IRacingId; }

    public string Number { get => model.Number; set => SetP(Number, value => model.Number = value, value); }

    public string DiscordId { get => model.DiscordId; set => SetP(DiscordId, value => model.DiscordId = value, value); }

    public TeamInfoModel Team { get => new() { Name = model.TeamName, }; set => SetP(Team, value => model.TeamName = value.Name, value); }

    public MemberViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService)
        : base(loggerFactory, apiService, new())
    {
    }

    public MemberViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, MemberModel model) 
        : base(loggerFactory, apiService, model)
    {           
    }

    public async Task<StatusResult> Save(CancellationToken cancellationToken)
    {
        if (CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var result = await CurrentLeague.Members()
                .WithId(MemberId)
                .Put(model, cancellationToken)
                .ConfigureAwait(false);
            if (result.Success && result.Content is MemberModel member)
            {
                SetModel(member);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
