namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class EventModel : PutEventModel
{
    /// <summary>
    /// EventId
    /// </summary>
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long SeasonId { get; set; }
    /// <summary>
    /// Id of the schedule this event belongs to
    /// </summary>
    [DataMember]
    public long ScheduleId { get; set; }
    [DataMember]
    public bool HasResult { get; set; }
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    [DataMember]
    public string ConfigName { get; set; } = string.Empty;
    [DataMember(EmitDefaultValue = false)]
    public SimSessionDetailsModel? SimSessionDetails { get; set; }
}
