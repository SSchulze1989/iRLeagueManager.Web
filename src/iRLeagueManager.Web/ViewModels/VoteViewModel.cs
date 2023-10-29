using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using Markdig.Extensions.Yaml;

namespace iRLeagueManager.Web.ViewModels;

public sealed class VoteViewModel : LeagueViewModelBase<VoteViewModel, VoteModel>
{
    public VoteViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, VoteModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long Id => model.Id;
    public long? VoteCategoryId { get => model.VoteCategoryId; set => SetP(model.VoteCategoryId, value => model.VoteCategoryId = value, value); }
    public string? VoteCategoryText { get => model.VoteCategoryText; }
    public string Description { get => model.Description; set => SetP(model.Description, value => model.Description = value, value); }
    public MemberInfoModel? MemberAtFault { get => model.MemberAtFault; set => SetP(model.MemberAtFault, value => model.MemberAtFault = value, value); }
    public TeamInfoModel? TeamAtFault { get => model.TeamAtFault; set => SetP(model.TeamAtFault, value => model.TeamAtFault = value, value); }

    public long? MemberAtFaultId
    {
        get => model.MemberAtFault?.MemberId;
        set
        {
            if (value is null)
            {
                MemberAtFault = null;
                return;
            }
            if (MemberAtFault?.MemberId != value)
            {
                MemberAtFault = new MemberInfoModel()
                {
                    MemberId = value.Value,
                };
            }
            OnPropertyChanged();
        }
    }

    public long? TeamAtFaultId
    {
        get => TeamAtFault?.TeamId;
        set => SetP(TeamAtFault, value => TeamAtFault = value, value == null ? default : new()
        {
            TeamId = value.Value,
        });
    }
}
