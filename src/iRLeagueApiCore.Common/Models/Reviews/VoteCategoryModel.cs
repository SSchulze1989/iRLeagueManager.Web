namespace iRLeagueApiCore.Common.Models.Reviews;

[DataContract]
public class VoteCategoryModel : PutVoteCategoryModel
{
    [DataMember]
    public long Id { get; set; }
}