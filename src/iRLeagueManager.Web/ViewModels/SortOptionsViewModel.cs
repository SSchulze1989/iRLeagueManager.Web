using iRLeagueApiCore.Common.Enums;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SortOptionsViewModel : LeagueViewModelBase<SortOptionsViewModel, IList<SortOptions>>
{
    public SortOptionsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new List<SortOptions>())
    {
    }

    public SortOptionsViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, IList<SortOptions> model) :
        base(loggerFactory, apiService, model)
    {
        options = new();
    }

    private ObservableCollection<SortOptionViewModel> options;
    public ObservableCollection<SortOptionViewModel> Options { get => options; set => Set(ref options, value); }

    public override void SetModel(IList<SortOptions> model)
    {
        base.SetModel(model);
        Options = new(model.Select(x => new SortOptionViewModel(x)).ToList());
    }

    public void AddOption(SortValue value, SortDirection direction)
    {
        var option = new SortOptionViewModel(new());
        option.SortValue = value;
        option.SortDirection = direction;
        Options.Add(option);
    }

    public void RemoveOption(SortOptionViewModel option)
    {
        Options.Remove(option);
    }

    public override IList<SortOptions> GetModel()
    {
        return Options.Select(x => x.GetModel()).ToList();
    }
}

public enum SortValue
{
    Pos,
    PosChg,
    StartPos,
    RacePts,
    PenPts,
    BonusPts,
    TotalPts,
    TotalPtsNoBonus,
    TotalPtsNoPenalty,
    Intvl,
    ComplLaps,
    LeadLaps,
    Incs,
    FastLap,
    QualLap,
    FinalPos,
}

public enum SortDirection
{
    Ascending,
    Descending,
}
