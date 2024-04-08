using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public class CommentHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public CommentHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<ReviewCommentEntity?> GetCommentEntityAsync(long commentId, CancellationToken cancellationToken)
    {
        return await dbContext.ReviewComments
            .Include(x => x.ReviewCommentVotes)
            .Where(x => x.CommentId == commentId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    protected virtual async Task<ReviewCommentEntity> MapToReviewCommentEntityAsync(LeagueUser user, PostReviewCommentModel postComment,
        ReviewCommentEntity commentEntity, CancellationToken cancellationToken)
    {
        commentEntity.Text = postComment.Text;
        commentEntity.ReviewCommentVotes = await MapCommentVoteList(postComment.Votes, commentEntity.ReviewCommentVotes, cancellationToken);
        return UpdateVersionEntity(user, commentEntity);
    }

    protected virtual async Task<ReviewCommentEntity> MapToReviewCommentEntityAsync(LeagueUser user, PutReviewCommentModel putComment,
        ReviewCommentEntity commentEntity, CancellationToken cancellationToken)
    {
        return await MapToReviewCommentEntityAsync(user, (PostReviewCommentModel)putComment, commentEntity, cancellationToken);
    }

    protected virtual async Task<ReviewCommentVoteEntity> MapToCommentVoteEntityAsync(VoteModel voteModel, ReviewCommentVoteEntity voteEntity,
        CancellationToken cancellationToken)
    {
        voteEntity.Description = voteModel.Description;
        voteEntity.MemberAtFault = await GetMemberEntityAsync(voteModel.MemberAtFault?.MemberId, cancellationToken);
        voteEntity.TeamAtFault = await GetTeamEntityAsync(voteModel.TeamAtFault?.TeamId, cancellationToken);
        voteEntity.VoteCategory = await GetVoteCategoryEntityAsync(voteEntity.LeagueId, voteModel.VoteCategoryId, cancellationToken);
        return voteEntity;
    }

    protected virtual async Task<ICollection<ReviewCommentVoteEntity>> MapCommentVoteList(IEnumerable<VoteModel> voteModels,
        ICollection<ReviewCommentVoteEntity> voteEntities, CancellationToken cancellationToken)
    {
        // Map votes
        foreach (var voteModel in voteModels)
        {
            var voteEntity = voteEntities
                .FirstOrDefault(x => x.ReviewVoteId == voteModel.Id && voteModel.Id != 0);
            if (voteEntity == null)
            {
                voteEntity = new ReviewCommentVoteEntity();
                voteEntities.Add(voteEntity);
            }
            await MapToCommentVoteEntityAsync(voteModel, voteEntity, cancellationToken);
        }
        // Delete votes that are no longer in source collection
        var deleteVotes = voteEntities
            .Where(x => voteModels.Any(y => y.Id == x.ReviewVoteId) == false);
        foreach (var deleteVote in deleteVotes)
        {
            dbContext.Remove(deleteVote);
        }
        return voteEntities;
    }

    protected virtual async Task<ReviewCommentModel?> MapToReviewCommentModelAsync(long commentId, CancellationToken cancellationToken)
    {
        return await dbContext.ReviewComments
            .Where(x => x.CommentId == commentId)
            .Select(MapToReviewCommentModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected Expression<Func<ReviewCommentEntity, ReviewCommentModel>> MapToReviewCommentModelExpression => comment => new ReviewCommentModel()
    {
        LeagueId = comment.LeagueId,
        CommentId = comment.CommentId,
        AuthorName = comment.AuthorName,
        AuthorUserId = comment.AuthorUserId,
        Date = TreatAsUTCDateTime(comment.Date),
        ReviewId = comment.ReviewId.GetValueOrDefault(),
        Text = comment.Text,
        Votes = comment.ReviewCommentVotes.Select(vote => new VoteModel()
        {
            Id = vote.ReviewVoteId,
            Description = vote.Description,
            VoteCategoryId = vote.VoteCategoryId.GetValueOrDefault(),
            VoteCategoryText = vote.VoteCategory.Text,
            MemberAtFault = vote.MemberAtFault == null ? default : new()
            {
                MemberId = vote.MemberAtFault.Id,
                FirstName = vote.MemberAtFault.Firstname,
                LastName = vote.MemberAtFault.Lastname
            },
            TeamAtFault = vote.TeamAtFault == null ? default : new()
            {
                TeamId = vote.TeamAtFault.TeamId,
                Name = vote.TeamAtFault.Name,
                TeamColor = vote.TeamAtFault.TeamColor,
            },
        }).ToList(),
        CreatedByUserId = comment.CreatedByUserId,
        CreatedByUserName = comment.CreatedByUserName,
        CreatedOn = TreatAsUTCDateTime(comment.CreatedOn),
        LastModifiedByUserId = comment.LastModifiedByUserId,
        LastModifiedByUserName = comment.LastModifiedByUserName,
        LastModifiedOn = TreatAsUTCDateTime(comment.LastModifiedOn),
    };
}
