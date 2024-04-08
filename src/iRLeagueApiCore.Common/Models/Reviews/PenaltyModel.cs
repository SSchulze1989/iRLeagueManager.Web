namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class PenaltyModel : PutPenaltyModel
{
    [DataMember]
    public long ResultRowId { get; set; }
    [DataMember]
    public long? AddPenaltyId { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public long? ReviewId { get; set; }
    [DataMember(EmitDefaultValue = false)]
    public long? ReviewVoteId { get; set; }
    [DataMember]
    public long EventId { get; set; }
    [DataMember]
    public int SessionNr { get; set; }
    [DataMember]
    public string SessionName { get; set; } = string.Empty;
    [DataMember]
    public MemberInfoModel? Member { get; set; }
    [DataMember]
    public TeamInfoModel? Team { get; set; }
}
