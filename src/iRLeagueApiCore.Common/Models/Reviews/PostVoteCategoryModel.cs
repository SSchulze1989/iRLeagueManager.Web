namespace iRLeagueApiCore.Common.Models.Reviews;

[DataContract]
public class PostVoteCategoryModel
{
    [DataMember]
    public string Text { get; set; } = string.Empty;
    [DataMember]
    public int Index { get; set; }
    [DataMember]
    public int DefaultPenalty { get; set; }
}