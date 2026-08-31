using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Globalization;

namespace iRLeagueManager.Web.ViewModels;

public class FilterConditionViewModel : LeagueViewModelBase<FilterConditionViewModel, FilterConditionModel>
{
    public FilterConditionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService, new())
    {
    }

    public FilterConditionViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, FilterConditionModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public string ColumnPropertyName
    {
        get => model.ColumnPropertyName;
        set
        {
            if (SetP(model.ColumnPropertyName, value => model.ColumnPropertyName = value, value))
            {
                UpdateFilterType();
            }
        }

    }
    public ComparatorType Comparator { get => model.Comparator; set => SetP(model.Comparator, value => model.Comparator = value, value); }
    public FilterType FilterType { get => model.FilterType; private set => SetP(model.FilterType, value => model.FilterType = value, value); }
    public IEnumerable<string> FilterValues { get => model.FilterValues; set => SetP(model.FilterValues, value => model.FilterValues = value.ToList(), value); }
    public string Value
    {
        get => ConvertFromValue(model.FilterValues.FirstOrDefault() ?? string.Empty, ColumnPropertyName);
        set => SetP(model.FilterValues.FirstOrDefault() ?? string.Empty, value => model.FilterValues = new[] { value }.ToList(), ConvertToValue(value, ColumnPropertyName));
    }
    public MatchedValueAction Action { get => model.Action; set => SetP(model.Action, value => model.Action = value, value); }
    public bool DisplayValueField => (Comparator is not ComparatorType.Min and not ComparatorType.Max);

    protected override void SetModel(FilterConditionModel model)
    {
        base.SetModel(model);
        UpdateFilterType();
    }

    private void UpdateFilterType()
    {
        FilterType = ColumnPropertyName switch
        {
            "Member" => FilterType.Member,
            "Team" => FilterType.Team,
            "Count" => FilterType.Count,
            _ => FilterType.ColumnProperty,
        };
    }

    private static string ConvertFromValue(string filterValue, string? columnProperty)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
        {
            return filterValue;
        }
        return columnProperty switch
        {
            nameof(ResultRowModel.CompletedPct) => ((double)Convert.ChangeType(filterValue, typeof(double), CultureInfo.InvariantCulture) * 100).ToString(CultureInfo.InvariantCulture),
            _ => filterValue
        };
    }

    private static string ConvertToValue(string value, string? columnProperty)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        return columnProperty switch
        {
            nameof(ResultRowModel.CompletedPct) => ((double)Convert.ChangeType(value, typeof(double), CultureInfo.InvariantCulture) / 100).ToString(CultureInfo.InvariantCulture),
            _ => value
        };
    }
}
