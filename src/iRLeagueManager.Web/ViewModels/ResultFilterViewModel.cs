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
    public IList<string> FilterValues { get => (IList<string>)model.FilterValues; set => SetP(model.FilterValues, value => model.FilterValues = value, value); }
    public string Value 
    { 
        get => ConvertFromValue(model.FilterValues.FirstOrDefault() ?? string.Empty, ColumnPropertyName); 
        set => SetP(model.FilterValues.FirstOrDefault() ?? string.Empty, value => model.FilterValues = new[] { ConvertToValue(value, ColumnPropertyName) }.ToList(), value); }
    public MatchedValueAction Action { get => model.Action; set => SetP(model.Action, value => model.Action = value, value); }

    public override void SetModel(ResultFilterModel model)
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

    private static string ConvertFromValue(string filterValue, string? columnProperty) => columnProperty switch
    {
        nameof(ResultRowModel.CompletedPct) => ((double)Convert.ChangeType(filterValue, typeof(double), CultureInfo.InvariantCulture) * 100).ToString(),
        _ => filterValue
    };

    private static string ConvertToValue(string value, string? columnProperty) => columnProperty switch
    {
        nameof(ResultRowModel.CompletedPct) => ((double)Convert.ChangeType(value, typeof(double), CultureInfo.InvariantCulture) / 100).ToString(),
        _ => value
    };
}
