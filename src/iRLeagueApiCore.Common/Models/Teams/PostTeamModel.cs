namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostTeamModel
{
    [DataMember]
    public string Name { get; set; } = string.Empty;
    [DataMember]
    public string Profile { get; set; } = string.Empty;
    [DataMember]
    public string TeamColor { get; set; } = string.Empty;
    [DataMember]
    public string TeamHomepage { get; set; } = string.Empty;
    [DataMember]
    public long? IRacingTeamId { get; set; } 
    [DataMember]
    public ICollection<MemberInfoModel> Members { get; set; } = new List<MemberInfoModel>();
}
