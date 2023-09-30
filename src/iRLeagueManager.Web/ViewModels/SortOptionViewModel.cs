using iRLeagueApiCore.Common.Enums;
using MudBlazor;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SortOptionViewModel : MvvmBlazor.ViewModel.ViewModelBase
{
    private SortOptions model;

    public SortOptionViewModel(SortOptions model)
    {
        SetModel(model);
    }

    private SortValue sortValue;
    public SortValue SortValue
    {
        get => sortValue;
        set
        {
            if (Set(ref sortValue, value))
            {
                model = GetSortOptions(sortValue, sortDirection);
            }
        }
    }

    private SortDirection sortDirection;
    public SortDirection SortDirection
    {
        get => sortDirection;
        set
        {
            if (Set(ref sortDirection, value))
            {
                model = GetSortOptions(sortValue, sortDirection);
            }
        }
    }

    private static (SortValue sortValue, SortDirection direction) GetSortValue(SortOptions value)
    {
        return value switch
        {
            SortOptions.PosAsc => (SortValue.Pos, SortDirection.Ascending),
            SortOptions.PosDesc => (SortValue.Pos, SortDirection.Descending),
            SortOptions.PosChgAsc => (SortValue.PosChg, SortDirection.Ascending),
            SortOptions.PosChgDesc => (SortValue.PosChg, SortDirection.Descending),
            SortOptions.StartPosAsc => (SortValue.StartPos, SortDirection.Ascending),
            SortOptions.StartPosDesc => (SortValue.StartPos, SortDirection.Descending),
            SortOptions.RacePtsAsc => (SortValue.RacePts, SortDirection.Ascending),
            SortOptions.RacePtsDesc => (SortValue.RacePts, SortDirection.Descending),
            SortOptions.PenPtsAsc => (SortValue.PenPts, SortDirection.Ascending),
            SortOptions.PenPtsDesc => (SortValue.PenPts, SortDirection.Descending),
            SortOptions.BonusPtsAsc => (SortValue.BonusPts, SortDirection.Ascending),
            SortOptions.BonusPtsDesc => (SortValue.BonusPts, SortDirection.Descending),
            SortOptions.TotalPtsAsc => (SortValue.TotalPts, SortDirection.Ascending),
            SortOptions.TotalPtsDesc => (SortValue.TotalPts, SortDirection.Descending),
            SortOptions.IntvlAsc => (SortValue.Intvl, SortDirection.Ascending),
            SortOptions.IntvlDesc => (SortValue.Intvl, SortDirection.Descending),
            SortOptions.ComplLapsAsc => (SortValue.ComplLaps, SortDirection.Ascending),
            SortOptions.ComplLapsDesc => (SortValue.ComplLaps, SortDirection.Descending),
            SortOptions.LeadLapsAsc => (SortValue.LeadLaps, SortDirection.Ascending),
            SortOptions.LeadLapsDesc => (SortValue.LeadLaps, SortDirection.Descending),
            SortOptions.IncsAsc => (SortValue.Incs, SortDirection.Ascending),
            SortOptions.IncsDesc => (SortValue.Incs, SortDirection.Descending),
            SortOptions.FastLapAsc => (SortValue.FastLap, SortDirection.Ascending),
            SortOptions.FastLapDesc => (SortValue.FastLap, SortDirection.Descending),
            SortOptions.QualLapAsc => (SortValue.QualLap, SortDirection.Ascending),
            SortOptions.QualLapDesc => (SortValue.QualLap, SortDirection.Descending),
            SortOptions.FinPosAsc => (SortValue.FinalPos, SortDirection.Ascending),
            SortOptions.FinPosDesc => (SortValue.FinalPos, SortDirection.Descending),
            SortOptions.TotalPtsWoBonusAsc => (SortValue.TotalPtsNoBonus, SortDirection.Ascending),
            SortOptions.TotalPtsWoBonusDesc => (SortValue.TotalPtsNoBonus, SortDirection.Descending),
            SortOptions.TotalPtsWoPenaltyAsc => (SortValue.TotalPtsNoPenalty, SortDirection.Ascending),
            SortOptions.TotalPtsWoPenaltyDesc => (SortValue.TotalPtsNoPenalty, SortDirection.Descending),
            SortOptions.LastRaceOrderAsc => (SortValue.LastRaceOrder, SortDirection.Ascending),
            SortOptions.LastRaceOrderDesc => (SortValue.LastRaceOrder, SortDirection.Descending),
            SortOptions.WinsAsc => (SortValue.Wins, SortDirection.Ascending),
            SortOptions.WinsDesc => (SortValue.Wins, SortDirection.Descending),
            SortOptions.Top3Asc => (SortValue.Top3, SortDirection.Ascending),
            SortOptions.Top3Desc => (SortValue.Top3, SortDirection.Descending),
            SortOptions.Top5Asc => (SortValue.Top5, SortDirection.Ascending),
            SortOptions.Top5Desc => (SortValue.Top5, SortDirection.Descending),
            SortOptions.Top10Asc => (SortValue.Top10, SortDirection.Ascending),
            SortOptions.Top10Desc => (SortValue.Top10, SortDirection.Descending),
            SortOptions.RacesAsc => (SortValue.Races, SortDirection.Ascending),
            SortOptions.RacesDesc => (SortValue.Races, SortDirection.Descending),
            SortOptions.RacesCountedAsc => (SortValue.RacesCounted, SortDirection.Ascending),
            SortOptions.RacesCountedDesc => (SortValue.RacesCounted, SortDirection.Descending),
            SortOptions.RacesScoredAsc => (SortValue.RacesScored, SortDirection.Ascending),
            SortOptions.RacesScoredDesc => (SortValue.RacesScored, SortDirection.Descending),
            SortOptions.RacesInPointsAsc => (SortValue.RacesInPoints, SortDirection.Ascending),
            SortOptions.RacesInPointsDesc => (SortValue.RacesInPoints, SortDirection.Descending),
            _ => (SortValue.TotalPts, SortDirection.Descending),
        };
    }

    private static SortOptions GetSortOptions(SortValue sortValue, SortDirection direction)
    {
        return (sortValue, direction) switch
        {
            (SortValue.Pos, SortDirection.Ascending) => SortOptions.PosAsc,
            (SortValue.Pos, SortDirection.Descending) => SortOptions.PosDesc,
            (SortValue.PosChg, SortDirection.Ascending) => SortOptions.PosChgAsc,
            (SortValue.PosChg, SortDirection.Descending) => SortOptions.PosChgDesc,
            (SortValue.StartPos, SortDirection.Ascending) => SortOptions.StartPosAsc,
            (SortValue.StartPos, SortDirection.Descending) => SortOptions.StartPosDesc,
            (SortValue.RacePts, SortDirection.Ascending) => SortOptions.RacePtsAsc,
            (SortValue.RacePts, SortDirection.Descending) => SortOptions.RacePtsDesc,
            (SortValue.PenPts, SortDirection.Ascending) => SortOptions.PenPtsAsc,
            (SortValue.PenPts, SortDirection.Descending) => SortOptions.PenPtsDesc,
            (SortValue.BonusPts, SortDirection.Ascending) => SortOptions.BonusPtsAsc,
            (SortValue.BonusPts, SortDirection.Descending) => SortOptions.BonusPtsDesc,
            (SortValue.TotalPts, SortDirection.Ascending) => SortOptions.TotalPtsAsc,
            (SortValue.TotalPts, SortDirection.Descending) => SortOptions.TotalPtsDesc,
            (SortValue.Intvl, SortDirection.Ascending) => SortOptions.IntvlAsc,
            (SortValue.Intvl, SortDirection.Descending) => SortOptions.IntvlDesc,
            (SortValue.ComplLaps, SortDirection.Ascending) => SortOptions.ComplLapsAsc,
            (SortValue.ComplLaps, SortDirection.Descending) => SortOptions.ComplLapsDesc,
            (SortValue.LeadLaps, SortDirection.Ascending) => SortOptions.LeadLapsAsc,
            (SortValue.LeadLaps, SortDirection.Descending) => SortOptions.LeadLapsDesc,
            (SortValue.Incs, SortDirection.Ascending) => SortOptions.IncsAsc,
            (SortValue.Incs, SortDirection.Descending) => SortOptions.IncsDesc,
            (SortValue.FastLap, SortDirection.Ascending) => SortOptions.FastLapAsc,
            (SortValue.FastLap, SortDirection.Descending) => SortOptions.FastLapDesc,
            (SortValue.QualLap, SortDirection.Ascending) => SortOptions.QualLapAsc,
            (SortValue.QualLap, SortDirection.Descending) => SortOptions.QualLapDesc,
            (SortValue.FinalPos, SortDirection.Ascending) => SortOptions.FinPosAsc,
            (SortValue.FinalPos, SortDirection.Descending) => SortOptions.FinPosDesc,
            (SortValue.TotalPtsNoBonus, SortDirection.Ascending) => SortOptions.TotalPtsWoBonusAsc,
            (SortValue.TotalPtsNoBonus, SortDirection.Descending) => SortOptions.TotalPtsWoBonusDesc,
            (SortValue.TotalPtsNoPenalty, SortDirection.Ascending) => SortOptions.TotalPtsWoPenaltyAsc,
            (SortValue.TotalPtsNoPenalty, SortDirection.Descending) => SortOptions.TotalPtsWoPenaltyDesc,
            (SortValue.Wins, SortDirection.Ascending) => SortOptions.WinsAsc,
            (SortValue.Wins, SortDirection.Descending) => SortOptions.WinsDesc,
            (SortValue.Top3, SortDirection.Ascending) => SortOptions.Top3Asc,
            (SortValue.Top3, SortDirection.Descending) => SortOptions.Top3Desc,
            (SortValue.Top5, SortDirection.Ascending) => SortOptions.Top5Asc,
            (SortValue.Top5, SortDirection.Descending) => SortOptions.Top5Desc,
            (SortValue.Top10, SortDirection.Ascending) => SortOptions.Top10Asc,
            (SortValue.Top10, SortDirection.Descending) => SortOptions.Top10Desc,
            (SortValue.Races, SortDirection.Ascending) => SortOptions.RacesAsc,
            (SortValue.Races, SortDirection.Descending) => SortOptions.RacesDesc,
            (SortValue.RacesCounted, SortDirection.Ascending) => SortOptions.RacesCountedAsc,
            (SortValue.RacesCounted, SortDirection.Descending) => SortOptions.RacesCountedDesc,
            (SortValue.RacesScored, SortDirection.Ascending) => SortOptions.RacesScoredAsc,
            (SortValue.RacesScored, SortDirection.Descending) => SortOptions.RacesScoredDesc,
            (SortValue.RacesInPoints, SortDirection.Ascending) => SortOptions.RacesInPointsAsc,
            (SortValue.RacesInPoints, SortDirection.Descending) => SortOptions.RacesInPointsDesc,
            _ => SortOptions.TotalPtsDesc,
        };
    }

    public void SetModel(SortOptions model)
    {
        this.model = model;
        (sortValue, sortDirection) = GetSortValue(model);
        OnPropertyChanged();
    }

    public SortOptions GetModel() => model;
}
