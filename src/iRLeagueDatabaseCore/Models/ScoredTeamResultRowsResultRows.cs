namespace iRLeagueDatabaseCore.Models;

public class ScoredTeamResultRowsResultRows
{
    public long LeagueId { get; set; }
    public long TeamResultRowRefId { get; set; }
    public long TeamParentRowRefId { get; set; }

    public virtual ScoredResultRowEntity TeamResultRow { get; set; }
    public virtual ScoredResultRowEntity TeamParentRow { get; set; }
}
