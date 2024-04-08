namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class ResultFilterModel
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long FilterOptionId { get; set; }
    [DataMember]
    public FilterConditionModel Condition { get; set; } = new();
}
