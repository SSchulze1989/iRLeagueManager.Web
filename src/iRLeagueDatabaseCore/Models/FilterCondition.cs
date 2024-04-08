using System.Runtime.Serialization;

namespace iRLeagueDatabaseCore.Models;

public sealed class FilterCondition
{
    public FilterType FilterType { get; set; }
    public string ColumnPropertyName { get; set; } = string.Empty;
    public ComparatorType Comparator { get; set; }
    public ICollection<string> FilterValues { get; set; } = new List<string>();
}