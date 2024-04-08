namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class AutoPenaltyConfiguration
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long PenaltyConfigId { get; set; }
    [DataMember]
    public string Description { get; set; } = string.Empty;
    [DataMember]
    public PenaltyType Type { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public int Points { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public TimeSpan Time { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public int Positions { get; set; }
    [DataMember]
    public ICollection<FilterConditionModel> Conditions { get; set; } = new List<FilterConditionModel>();
}
