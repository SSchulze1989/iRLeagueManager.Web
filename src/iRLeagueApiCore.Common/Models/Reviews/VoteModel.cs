namespace iRLeagueApiCore.Common.Models.Reviews;

[DataContract]
public class VoteModel
{
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    public long? VoteCategoryId { get; set; }
    [DataMember]
    public string? VoteCategoryText { get; set; }
    [DataMember]
    public string Description { get; set; } = string.Empty;
    [DataMember]
    public MemberInfoModel? MemberAtFault { get; set; }
    [DataMember]
    public TeamInfoModel? TeamAtFault { get; set; }
}
