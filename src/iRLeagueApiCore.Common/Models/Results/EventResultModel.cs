namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class EventResultModel
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long EventId { get; set; }
    [DataMember]
    public long ResultId { get; set; }
    [DataMember]
    public long SeasonId { get; set; }
    [DataMember]
    public string EventName { get; set; } = string.Empty;
    [DataMember]
    public string DisplayName { get; set; } = string.Empty;
    [DataMember]
    public DateTime Date { get; set; }
    [DataMember]
    public long TrackId { get; set; }
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    [DataMember]
    public string ConfigName { get; set; } = string.Empty;
    [DataMember]
    public int StrengthOfField { get; set; }
    [DataMember]
    public IEnumerable<ResultModel> SessionResults { get; set; } = Array.Empty<ResultModel>();
}
