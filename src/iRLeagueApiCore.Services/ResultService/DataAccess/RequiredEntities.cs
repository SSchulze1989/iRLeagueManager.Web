using iRLeagueDatabaseCore.Models;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

// TODO: find better name for this class
internal sealed class RequiredEntities
{
    public ICollection<MemberEntity> Members { get; set; } = Array.Empty<MemberEntity>();
    public ICollection<TeamEntity> Teams { get; set; } = Array.Empty<TeamEntity>();
    public ICollection<ScoredResultRowEntity> ScoredResultRows { get; set; } = Array.Empty<ScoredResultRowEntity>();
    public ICollection<AddPenaltyEntity> AddPenalties { get; set; } = Array.Empty<AddPenaltyEntity>();
    public ICollection<IncidentReviewEntity> Reviews { get; set; } = Array.Empty<IncidentReviewEntity>();
}
