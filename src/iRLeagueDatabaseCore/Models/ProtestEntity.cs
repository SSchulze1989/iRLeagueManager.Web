namespace iRLeagueDatabaseCore.Models;

public partial class ProtestEntity
{
    public ProtestEntity()
    {
        InvolvedMembers = new HashSet<LeagueMemberEntity>();
    }

    public long LeagueId { get; set; }
    public long ProtestId { get; set; }
    public long SessionId { get; set; }
    public long AuthorMemberId { get; set; }

    public string FullDescription { get; set; }
    public string OnLap { get; set; }
    public string Corner { get; set; }

    public virtual LeagueMemberEntity Author { get; set; }
    public virtual SessionEntity Session { get; set; }
    public virtual ICollection<LeagueMemberEntity> InvolvedMembers { get; set; }
}

public sealed class ProtestEntityConfiguration : IEntityTypeConfiguration<ProtestEntity>
{
    public void Configure(EntityTypeBuilder<ProtestEntity> entity)
    {
        entity.ToTable("Protests");

        entity.HasKey(e => new { e.LeagueId, e.ProtestId });

        entity.HasAlternateKey(e => e.ProtestId);

        entity.Property(e => e.ProtestId)
            .ValueGeneratedOnAdd();

        entity.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => new { p.LeagueId, p.AuthorMemberId });

        entity.HasOne(p => p.Session)
            .WithMany()
            .HasForeignKey(p => new { p.LeagueId, p.SessionId });

        entity.HasMany(p => p.InvolvedMembers)
            .WithMany(d => d.ProtestsInvolved)
            .UsingEntity<Protests_LeagueMembers>(
                right => right.HasOne(e => e.Member)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.MemberId }),
                left => left.HasOne(e => e.Protest)
                    .WithMany()
                    .HasForeignKey(e => new { e.LeagueId, e.ProtestId }));
    }
}
