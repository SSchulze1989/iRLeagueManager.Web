namespace iRLeagueApiCore.Common.Models;

[DataContract]
public class PointRuleModel : PutPointRuleModel
{
    [DataMember]
    public long LeagueId { get; set; }
    [DataMember]
    public long PointRuleId { get; set; }
}
