using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ProtestViewModel : LeagueViewModelBase<ProtestViewModel, ProtestModel>
{
    public ProtestViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ProtestModel model) : 
        base(loggerFactory, apiService, model)
    {
    }

    public long ProtestId => model.ProtestId;
    public long EventId => model.EventId;
    public long SessionId => model.SessionId;
    public int SessionNr => model.SessionNr;
    public string SessionName => model.SessionName;
    public MemberInfoModel Author => model.Author;
    public string FullDescription { get => model.FullDescription; set => SetP(model.FullDescription, value => model.FullDescription = value, value); }
    public string OnLap { get => model.OnLap; set => SetP(model.OnLap, value => model.OnLap = value, value); }
    public string Corner { get => model.Corner; set => SetP(model.Corner, value => model.Corner = value, value); }
    public IList<MemberInfoModel> InvolvedMembers => (IList<MemberInfoModel>)model.InvolvedMembers;

    public async Task<StatusResult> DeleteAsync(CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague
                .Protests()
                .WithId(ProtestId)
                .Delete(cancellationToken);
            var result = await request;
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }
}
