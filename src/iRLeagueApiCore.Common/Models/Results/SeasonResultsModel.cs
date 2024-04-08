namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class SeasonEventResultModel
{
    [DataMember]
    public long EventId { get; set; }
    [DataMember]
    public string EventName { get; set; } = string.Empty;
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    [DataMember]
    public string ConfigName { get; set; } = string.Empty;
    [DataMember]
    public IEnumerable<EventResultModel> EventResults { get; set; } = Array.Empty<EventResultModel>();
}
