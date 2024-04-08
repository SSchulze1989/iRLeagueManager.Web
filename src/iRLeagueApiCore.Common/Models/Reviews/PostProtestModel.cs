namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PostProtestModel
{
    [DataMember]
    public long AuthorMemberId { get; set; }
    [DataMember]
    public string ConfirmIRacingId { get; set; } = string.Empty;
    [DataMember]
    public string FullDescription { get; set; } = string.Empty;
    [DataMember]
    public string OnLap { get; set; } = string.Empty;
    [DataMember]
    public string Corner { get; set; } = string.Empty;
    [DataMember]
    public IList<MemberInfoModel> InvolvedMembers { get; set; } = new List<MemberInfoModel>();
}
