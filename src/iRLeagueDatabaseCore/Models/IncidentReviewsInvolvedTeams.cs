
namespace iRLeagueDatabaseCore.Models;
public partial class IncidentReviewsInvolvedTeams
{
    public long LeagueId { get; set; }
    public long ReviewRefId { get; set; }
    public long TeamRefId { get; set; }

    public virtual IncidentReviewEntity Review { get; set; }
    public virtual TeamEntity Team { get; set; }
}
