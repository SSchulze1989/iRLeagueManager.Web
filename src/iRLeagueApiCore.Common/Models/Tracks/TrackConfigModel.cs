namespace iRLeagueApiCore.Common.Models.Tracks;

[DataContract]
public class TrackConfigModel
{
    /// <summary>
    /// Id of this configuration. The same as iracing track_id
    /// </summary>
    [DataMember]
    public long TrackId { get; set; }
    /// <summary>
    /// Name of the track (or track group)
    /// </summary>
    [DataMember]
    public string TrackName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the specific track configuration
    /// </summary>
    [DataMember]
    public string ConfigName { get; set; } = string.Empty;
    [DataMember]
    public int Turns { get; set; }
    [DataMember]
    public double Length { get; set; }
    [DataMember]
    public ConfigType Type { get; set; }
    [DataMember]
    public bool HasNightLighting { get; set; }
}
