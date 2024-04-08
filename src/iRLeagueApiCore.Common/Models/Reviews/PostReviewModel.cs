namespace iRLeagueApiCore.Common.Models.Reviews;

[DataContract]
public class PostReviewModel
{
    [DataMember]
    public string IncidentKind { get; set; } = string.Empty;
    [DataMember]
    public string FullDescription { get; set; } = string.Empty;
    [DataMember]
    public string OnLap { get; set; } = string.Empty;
    [DataMember]
    public string Corner { get; set; } = string.Empty;
    [DataMember]
    public TimeSpan TimeStamp { get; set; }
    [DataMember]
    public string IncidentNr { get; set; } = string.Empty;
    [DataMember]
    public ICollection<MemberInfoModel> InvolvedMembers { get; set; } = new List<MemberInfoModel>();
    [DataMember]
    public ICollection<TeamInfoModel> InvolvedTeams { get; set; } = new List<TeamInfoModel>();
    [DataMember]
    public string ResultText { get; set; } = string.Empty;
    [DataMember]
    public ICollection<VoteModel> VoteResults { get; set; } = new List<VoteModel>();
}
