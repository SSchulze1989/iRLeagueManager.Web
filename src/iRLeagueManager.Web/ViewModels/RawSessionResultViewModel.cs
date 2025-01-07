using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class RawSessionResultViewModel : LeagueViewModelBase<RawSessionResultViewModel, RawSessionResultModel>
{
    public RawSessionResultViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, RawSessionResultModel model) 
        : base(loggerFactory, apiService, model)
    {
    }

    public long SessionId { get => model.SessionId; }

    private List<RawResultRowViewModel> resultRows = [];
    public List<RawResultRowViewModel> ResultRows { get => resultRows; set => Set(ref resultRows, value); }

    protected override void SetModel(RawSessionResultModel model)
    {
        base.SetModel(model);
        ResultRows = model.ResultRows
            .Select(x => new RawResultRowViewModel(LoggerFactory, ApiService, x))
            .ToList();
    }
}
