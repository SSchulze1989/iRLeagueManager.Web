namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class ChampSeasonInfoModel
{
    [DataMember]
    public long ChampSeasonId { get; set; }
    [DataMember]
    public long ChampionshipId { get; set; }
    [DataMember]
    public long SeasonId { get; set; }
    [DataMember]
    public string ChampionshipName { get; set; } = string.Empty;
    [DataMember]
    public string SeasonName { get; set; } = string.Empty;
}