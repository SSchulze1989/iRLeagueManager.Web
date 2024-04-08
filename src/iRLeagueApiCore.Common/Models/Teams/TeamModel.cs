namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class TeamModel : PutTeamModel
{
    [DataMember]
    public long TeamId { get; set; }
}
