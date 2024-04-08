namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PutProtestModel
{
    [DataMember]
    public string FullDescription { get; set; } = string.Empty;
    [DataMember]
    public string OnLap { get; set; } = string.Empty;
    [DataMember]
    public string Corner { get; set; } = string.Empty;
    [DataMember]
    public ICollection<MemberInfoModel> InvolvedMembers { get; set; } = new List<MemberInfoModel>();
}
