namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class ChampionshipModel : PutChampionshipModel
{
    [DataMember]
    public long ChampionshipId { get; set; }
    [DataMember]
    public IReadOnlyCollection<ChampSeasonInfoModel> Seasons { get; set; } = Array.Empty<ChampSeasonInfoModel>();
}
