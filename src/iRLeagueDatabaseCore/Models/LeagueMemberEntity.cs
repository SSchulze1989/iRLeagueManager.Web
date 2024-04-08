#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class LeagueMemberEntity
{
    public LeagueMemberEntity()
    {
    }

    public long MemberId { get; set; }
    public long LeagueId { get; set; }
    public long? TeamId { get; set; }

    public virtual MemberEntity Member { get; set; }
    public virtual LeagueEntity League { get; set; }
    public virtual TeamEntity Team { get; set; }
    public virtual IEnumerable<ProtestEntity> ProtestsInvolved { get; set; }
}

public class LeagueMemberEntityConfiguration : IEntityTypeConfiguration<LeagueMemberEntity>
{
    public void Configure(EntityTypeBuilder<LeagueMemberEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.MemberId });

        entity.HasIndex(e => e.MemberId);

        entity.HasOne(e => e.League)
            .WithMany(e => e.LeagueMembers)
            .HasForeignKey(e => e.LeagueId);

        entity.HasOne(e => e.Team)
            .WithMany(e => e.Members)
            .HasForeignKey(e => new { e.LeagueId, e.TeamId });
    }
}
