using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Models;
using System.Globalization;
using System.Reflection;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class ColumnValueRowFilter : RowFilter<ResultRowCalculationResult>
{
    public bool AllowForEach { get; private set; }

    /// <summary>
    /// Create an instance of a <see cref="ColumnValueRowFilter"/>
    /// </summary>
    /// <param name="propertyName">Name of the property matching the desired column</param>
    /// <param name="comparator">Type of comparison to execute</param>
    /// <param name="values">Values corresponding to comparator</param>
    /// <param name="action">Action to perform: Keep / Remove</param>
    /// <param name="allowForEach">Enable multiplication of rows using "ForEach" comparator</param>
    /// <exception cref="ArgumentException"></exception>
    public ColumnValueRowFilter(string propertyName, ComparatorType comparator, IEnumerable<string> values, MatchedValueAction action, bool allowForEach = false)
    {
        Action = action;
        AllowForEach = allowForEach;
        try
        {
            ColumnProperty = GetColumnPropertyInfo(propertyName);
        }
        catch (InvalidOperationException ex)
        {
            throw new ArgumentException($"Parameter value {propertyName} did not target a valid column property on type {typeof(ResultRowCalculationResult)}",
                nameof(propertyName), ex);
        }
        Comparator = comparator;
        CompareFunc = GetCompareFunction(comparator);
        try
        {
            FilterValues = GetFilterValuesOfType(ColumnProperty.PropertyType, values).ToList();
        }
        catch (Exception ex) when (ex is InvalidCastException ||
                                   ex is FormatException ||
                                   ex is OverflowException ||
                                   ex is ArgumentNullException)
        {
            throw new ArgumentException($"Parameter was not of type {ColumnProperty.PropertyType} of column property {ColumnProperty.Name}", nameof(values), ex);
        }
    }

    public PropertyInfo ColumnProperty { get; }
    public ComparatorType Comparator { get; }
    private Func<IComparable?, IEnumerable<IComparable>, bool> CompareFunc { get; }
    public IEnumerable<IComparable> FilterValues { get; }
    public MatchedValueAction Action { get; }

    public override IEnumerable<T> FilterRows<T>(IEnumerable<T> rows)
    {        
        var match = rows.Where(x => MatchFilterValues(x, ColumnProperty, FilterValues, CompareFunc));
        if (Comparator == ComparatorType.ForEach && AllowForEach)
        {
            // special handling for ForEach --> duplicate rows by multiple of values
            match = MultiplyRows(match, ColumnProperty, FilterValues); 
        }
        return Action switch
        {
            MatchedValueAction.Keep => match,
            MatchedValueAction.Remove => rows.Except(match),
            _ => rows,
        };
    }

    private static bool MatchFilterValues(ResultRowCalculationResult row, PropertyInfo property, IEnumerable<IComparable> filterValues, Func<IComparable?, IEnumerable<IComparable>, bool> compare)
    {
        var value = (IComparable?)property.GetValue(row);
        return compare(value, filterValues);
    }

    private static PropertyInfo GetColumnPropertyInfo(string propertyName)
    {
        var sourceType = typeof(ResultRowCalculationResult);
        var propertyInfo = sourceType.GetProperty(propertyName)
            ?? throw new InvalidOperationException($"{typeof(ResultRowCalculationResult)} does not have a property with name {propertyName}");
        if (typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType) == false)
        {
            throw new InvalidOperationException($"Column {propertyName} of type {typeof(ResultRowCalculationResult)} does not implement IComparable");
        }
        return propertyInfo;
    }

    private static IEnumerable<IComparable> GetFilterValuesOfType(Type type, IEnumerable<string> values)
    {
        if (type.Equals(typeof(TimeSpan)))
        {
            return values.Select(x => TimeSpan.Parse(x)).Cast<IComparable>();
        }
        return values.Select(x => Convert.ChangeType(x, type, CultureInfo.InvariantCulture)).Cast<IComparable>();
    }

    private static IEnumerable<T> MultiplyRows<T>(IEnumerable<T> rows, PropertyInfo property, 
        IEnumerable<IComparable> filterValues)
    {
        if (filterValues.Any() == false)
        {
            return rows;
        }
        var compareValue = Convert.ToDouble(filterValues.First());
        List<T> multipliedRows = new();
        foreach (var row in rows)
        {
            var value = Convert.ToDouble(property.GetValue(row));
            var count = (int)(value / compareValue);
            for (int i=0; i<count; i++)
            {
                multipliedRows.Add(row);
            }
        }
        return multipliedRows;
    }

    private static Func<IComparable?, IEnumerable<IComparable>, bool> GetCompareFunction(ComparatorType comparatorType)
        => comparatorType switch
        {
            ComparatorType.IsBigger => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == 1; },
            ComparatorType.IsBiggerOrEqual => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == 1 || c == 0; },
            ComparatorType.IsEqual => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == 0; },
            ComparatorType.IsSmallerOrEqual => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == -1 || c == 0; },
            ComparatorType.IsSmaller => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == -1; },
            ComparatorType.NotEqual => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == 1 || c == -1; },
            ComparatorType.InList => (x, y) => { var c = y.Any(z => x?.CompareTo(z) == 0); return c; },
            ComparatorType.ForEach => (x, y) => { var c = x?.CompareTo(y.FirstOrDefault()); return c == 1 || c == 0; },
            _ => (x, y) => true,
        };
}
