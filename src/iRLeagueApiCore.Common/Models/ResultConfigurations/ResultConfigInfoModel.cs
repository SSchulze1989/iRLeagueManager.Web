namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class ResultConfigInfoModel
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long ResultConfigId { get; set; }
    [DataMember]
    public long ChampSeasonId { get; set; }
    [DataMember]
    public string ChampionshipName { get; set; } = string.Empty;
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public string DisplayName { get; set; } = string.Empty;
    [DataMember]
    public bool IsDefaultConfig { get; set; }
}
