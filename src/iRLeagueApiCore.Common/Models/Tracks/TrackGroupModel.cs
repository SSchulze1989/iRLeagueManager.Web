namespace iRLeagueApiCore.Common.Models.Tracks;

[DataContract]
public class TrackGroupModel
{
    [DataMember]
    public long TrackGroupId { get; set; }
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    [DataMember]
    public IEnumerable<TrackConfigModel> Configs { get; set; } = Array.Empty<TrackConfigModel>();
}
