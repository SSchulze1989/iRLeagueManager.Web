using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class AutoPenaltyConfigViewModel : LeagueViewModelBase<AutoPenaltyConfigViewModel, AutoPenaltyConfiguration>
{
    public AutoPenaltyConfigViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, AutoPenaltyConfiguration model) 
        : base(loggerFactory, apiService, model)
    {
    }

    public long LeagueId => model.LeagueId;
    public long PenaltyConfigId => model.PenaltyConfigId;
    public string Description { get => model.Description; set => SetP(model.Description, value => model.Description = value, value); }
    public PenaltyType Type { get => model.Type; set => SetP(model.Type, value => model.Type = value, value); }
    public int Points { get => -model.Points; set => SetP(model.Points, value => model.Points = value, -value); }
    public TimeSpan Time { get => model.Time; set => SetP(model.Time, value => model.Time = value, value); }
    public int TimeSeconds { get => (int)model.Time.TotalSeconds; set => SetP(TimeSeconds, value => model.Time = TimeSpan.FromSeconds(value), value); }
    public int Positions { get => model.Positions; set => SetP(model.Positions, value => model.Positions = value, value); }
    public ICollection<FilterConditionModel> Conditions { get => model.Conditions; set => SetP(model.Conditions, value =>  model.Conditions = value, value); }
}
