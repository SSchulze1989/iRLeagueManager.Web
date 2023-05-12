using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class StandingConfigurationViewModel : LeagueViewModelBase<StandingConfigurationViewModel, StandingConfigModel>
{
    public StandingConfigurationViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, StandingConfigModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long StandingConfigId { get => model.StandingConfigId; set => SetP(model.StandingConfigId, value => model.StandingConfigId = value, value); }

    public string Name { get => model.Name; set => SetP(model.Name, value => model.Name = value, value); }

    public ResultKind ResultKind { get => model.ResultKind; set => SetP(model.ResultKind, value => model.ResultKind = value, value); }

    public bool UseCombinedResult { get => model.UseCombinedResult; set => SetP(model.UseCombinedResult, value => model.UseCombinedResult = value, value); }

    public int? WeeksCounted { get => model.WeeksCounted <= 0 ? null : model.WeeksCounted; set => SetP(model.WeeksCounted, value => model.WeeksCounted = value, value > 0 ? value.Value : 0); }
}
