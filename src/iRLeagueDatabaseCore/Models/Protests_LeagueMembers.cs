namespace iRLeagueDatabaseCore.Models;

public partial class Protests_LeagueMembers
{
    public long LeagueId { get; set; }
    public long ProtestId { get; set; }
    public long MemberId { get; set; }

    public virtual ProtestEntity Protest { get; set; }
    public virtual LeagueMemberEntity Member { get; set; }
}
