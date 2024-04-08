namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostPenaltyModel
{
    [DataMember]
    public string Lap { get; set; } = string.Empty;
    [DataMember]
    public string Corner { get; set; } = string.Empty;
    [DataMember]
    public string Reason { get; set; } = string.Empty;
    [DataMember]
    public PenaltyType Type { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public int Points { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public TimeSpan Time { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public int Positions { get; set; }
}
