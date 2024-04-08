namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class StandingConfigModel
{
    [DataMember]
    public long StandingConfigId { get; set; }
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public ResultKind ResultKind { get; set; }
    [DataMember]
    public bool UseCombinedResult { get; set; }
    [DataMember]
    public int WeeksCounted { get; set; }
    [DataMember]
    public ICollection<SortOptions> SortOptions { get; set; } = new List<SortOptions>();
}
