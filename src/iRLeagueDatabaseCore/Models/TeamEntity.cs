#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class TeamEntity : IVersionEntity
{
    public TeamEntity()
    {
        Members = new HashSet<LeagueMemberEntity>();
    }

    public long LeagueId { get; set; }
    public long TeamId { get; set; }
    public long? IRacingTeamId { get; set; }
    public string Name { get; set; }
    public string Profile { get; set; }
    public string TeamColor { get; set; }
    public string TeamHomepage { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public virtual ICollection<LeagueMemberEntity> Members { get; set; }
    public virtual LeagueEntity League { get; set; }
    public virtual IEnumerable<IncidentReviewEntity> InvolvedReviews { get; set; }
}

public class TeamEntityConfiguration : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.TeamId });

        entity.HasAlternateKey(e => e.TeamId);

        entity.Property(e => e.TeamId)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
    }
}
