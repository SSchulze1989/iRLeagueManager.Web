namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostSeasonModel
{
    [DataMember]
    public string SeasonName { get; set; } = string.Empty;
    [DataMember]
    public long? MainScoringId { get; set; }
    [DataMember]
    public bool HideComments { get; set; }
    [DataMember]
    public bool Finished { get; set; }
}
