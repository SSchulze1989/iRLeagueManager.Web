namespace iRLeagueApiCore.Common.Models.Results;

[DataContract]
public sealed class CarListModel
{
    [DataMember]
    public bool IsTeamEvent { get; set; }
    [DataMember]
    public IEnumerable<EventCarInfoModel> Cars { get; set; } = Array.Empty<EventCarInfoModel>();
}
