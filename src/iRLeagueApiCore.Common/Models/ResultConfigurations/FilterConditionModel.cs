namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class FilterConditionModel
{
    [DataMember]
    public FilterType FilterType { get; set; }
    [DataMember]
    public string ColumnPropertyName { get; set; } = string.Empty;
    [DataMember]
    public ComparatorType Comparator { get; set; }
    [DataMember]
    public ICollection<string> FilterValues { get; set; } = new List<string>();
    [DataMember]
    public MatchedValueAction Action { get; set; }
}
