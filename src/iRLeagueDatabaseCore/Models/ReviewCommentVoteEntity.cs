#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class ReviewCommentVoteEntity
{
    public long LeagueId { get; set; }
    public long ReviewVoteId { get; set; }
    public long CommentId { get; set; }
    public long? MemberAtFaultId { get; set; }
    public long? TeamAtFaultId { get; set; }
    public long? VoteCategoryId { get; set; }
    public string Description { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public virtual ReviewCommentEntity Comment { get; set; }
    public virtual VoteCategoryEntity VoteCategory { get; set; }
    public virtual MemberEntity MemberAtFault { get; set; }
    public virtual TeamEntity TeamAtFault { get; set;}
}

public class CommentReviewVoteEntityConfiguration : IEntityTypeConfiguration<ReviewCommentVoteEntity>
{
    public void Configure(EntityTypeBuilder<ReviewCommentVoteEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.ReviewVoteId });

        entity.HasAlternateKey(e => e.ReviewVoteId);

        entity.Property(e => e.ReviewVoteId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.CommentId);

        entity.HasIndex(e => e.VoteCategoryId);

        entity.HasIndex(e => e.MemberAtFaultId);

        entity.HasOne(d => d.Comment)
            .WithMany(p => p.ReviewCommentVotes)
            .HasForeignKey(d => new { d.LeagueId, d.CommentId });

        entity.HasOne(d => d.VoteCategory)
            .WithMany(p => p.CommentReviewVotes)
            .HasForeignKey(d => new { d.LeagueId, d.VoteCategoryId });

        entity.HasOne(d => d.MemberAtFault)
            .WithMany(p => p.CommentReviewVotes)
            .HasForeignKey(d => d.MemberAtFaultId);

        entity.HasOne(d => d.TeamAtFault)
            .WithMany()
            .HasForeignKey(d => new { d.LeagueId, d.TeamAtFaultId });
    }
}
