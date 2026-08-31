using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;
using System.Globalization;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ResultFilterViewModel : LeagueViewModelBase<ResultFilterViewModel, ResultFilterModel>
{
    public ResultFilterViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public ResultFilterViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultFilterModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long LeagueId => model.LeagueId;
    public long ResultsFilterId => model.FilterOptionId;
    public string ColumnPropertyName 
    { 
        get => model.Condition.ColumnPropertyName;
        set 
        {
            if (SetP(model.Condition.ColumnPropertyName, value => model.Condition.ColumnPropertyName = value, value))
            {
                UpdateFilterType();
            }
        }
        
    }
    public ComparatorType Comparator { get => model.Condition.Comparator; set => SetP(model.Condition.Comparator, value => model.Condition.Comparator = value, value); }
    public FilterType FilterType { get => model.Condition.FilterType; private set => SetP(model.Condition.FilterType, value => model.Condition.FilterType = value, value); }
    public IEnumerable<string> FilterValues { get => model.Condition.FilterValues; set => SetP(model.Condition.FilterValues, value => model.Condition.FilterValues = value.ToList(), value); }
    public string Value 
    { 
        get => ConvertFromValue(model.Condition.FilterValues.FirstOrDefault() ?? string.Empty, ColumnPropertyName); 
        set => SetP(model.Condition.FilterValues.FirstOrDefault() ?? string.Empty, value => model.Condition.FilterValues = new[] { value }.ToList(), ConvertToValue(value, ColumnPropertyName)); }
    public MatchedValueAction Action { get => model.Condition.Action; set => SetP(model.Condition.Action, value => model.Condition.Action = value, value); }

    protected override void SetModel(ResultFilterModel model)
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
