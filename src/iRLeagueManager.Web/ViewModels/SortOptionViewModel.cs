using iRLeagueApiCore.Common.Enums;
using iRLeagueManager.Web.Helpers;
using MudBlazor;

namespace iRLeagueManager.Web.ViewModels;

public sealed class SortOptionViewModel : ViewModelBase
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
        return value.ToSortValue();
    }

    private static SortOptions GetSortOptions(SortValue sortValue, SortDirection direction)
    {
        return (sortValue, direction).ToSortOption();
    }

    public void SetModel(SortOptions model)
    {
        this.model = model;
        (sortValue, sortDirection) = GetSortValue(model);
        OnPropertyChanged();
    }

    public SortOptions GetModel() => model;
}
