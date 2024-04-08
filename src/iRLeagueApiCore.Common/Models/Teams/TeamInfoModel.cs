namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class TeamInfoModel 
{
    [DataMember]
    public long TeamId { get; set; }
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public string TeamColor { get; set; } = string.Empty;
}
