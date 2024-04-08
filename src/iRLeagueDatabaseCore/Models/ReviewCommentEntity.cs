#nullable disable

namespace iRLeagueDatabaseCore.Models;

public partial class ReviewCommentEntity : IVersionEntity
{
    public ReviewCommentEntity()
    {
        ReviewCommentVotes = new HashSet<ReviewCommentVoteEntity>();
        Replies = new HashSet<ReviewCommentEntity>();
    }

    public long LeagueId { get; set; }
    public long CommentId { get; set; }
    public long? ReviewId { get; set; }
    public long? ReplyToCommentId { get; set; }
    /// <summary>
    /// Imported Id from old database
    /// Will be deleted after imports have finished
    /// </summary>
    public long? ImportId { get; set; }

    public DateTime? Date { get; set; }
    public string AuthorUserId { get; set; }
    public string AuthorName { get; set; }
    public string Text { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public int Version { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; }

    public virtual ReviewCommentEntity ReplyToComment { get; set; }
    public virtual IncidentReviewEntity Review { get; set; }
    public virtual ICollection<ReviewCommentVoteEntity> ReviewCommentVotes { get; set; }
    public virtual ICollection<ReviewCommentEntity> Replies { get; set; }
}

public class CommentBaseEntityConfiguation : IEntityTypeConfiguration<ReviewCommentEntity>
{
    public void Configure(EntityTypeBuilder<ReviewCommentEntity> entity)
    {
        entity.HasKey(e => new { e.LeagueId, e.CommentId });

        entity.HasAlternateKey(e => e.CommentId);

        entity.Property(e => e.CommentId)
            .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.ReplyToCommentId);

        entity.HasIndex(e => e.ReviewId);

        entity.Property(e => e.CreatedOn).HasColumnType("datetime");

        entity.Property(e => e.Date).HasColumnType("datetime");

        entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

        entity.HasOne(d => d.ReplyToComment)
            .WithMany(p => p.Replies)
            .HasForeignKey(d => new { d.LeagueId, d.ReplyToCommentId });

        entity.HasOne(d => d.Review)
            .WithMany(p => p.Comments)
            .HasForeignKey(d => new { d.LeagueId, d.ReviewId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
