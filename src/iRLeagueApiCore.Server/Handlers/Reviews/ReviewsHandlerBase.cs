using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public class ReviewsHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ReviewsHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<IncidentReviewEntity?> GetReviewEntity(long reviewId, CancellationToken cancellationToken)
    {
        return await dbContext.IncidentReviews
            .Include(x => x.InvolvedMembers)
            .Include(x => x.InvolvedTeams)
            .Include(x => x.AcceptedReviewVotes)
                .ThenInclude(x => x.ReviewPenaltys)
            .Where(x => x.ReviewId == reviewId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<IncidentReviewEntity> MapToReviewEntity(LeagueUser user, PostReviewModel postModel, IncidentReviewEntity reviewEntity, CancellationToken cancellationToken)
    {
        reviewEntity.AuthorName = user.Name;
        reviewEntity.AuthorUserId = user.Id;
        reviewEntity.Corner = postModel.Corner;
        reviewEntity.OnLap = postModel.OnLap;
        reviewEntity.FullDescription = postModel.FullDescription;
        reviewEntity.IncidentKind = postModel.IncidentKind;
        reviewEntity.IncidentNr = postModel.IncidentNr;
        reviewEntity.TimeStamp = postModel.TimeStamp;
        reviewEntity.InvolvedMembers = await GetMemberListAsync(postModel.InvolvedMembers.Select(x => x.MemberId), cancellationToken);
        reviewEntity.InvolvedTeams = await GetTeamListAsync(postModel.InvolvedTeams.Select(x => x.TeamId), cancellationToken);
        reviewEntity.ResultLongText = postModel.ResultText;
        reviewEntity.AcceptedReviewVotes = await MapToAcceptedVoteList(postModel.VoteResults, reviewEntity.AcceptedReviewVotes, cancellationToken);
        return UpdateVersionEntity(user, reviewEntity);
    }

    protected virtual async Task<AddPenaltyEntity?> GetAddPenaltyEntity(long addPenaltyId, CancellationToken cancellationToken)
    {
        return await dbContext.AddPenaltys
            .Where(x => x.AddPenaltyId == addPenaltyId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<AddPenaltyEntity> MapToAddPenaltyEntity(PostPenaltyModel postPenalty, AddPenaltyEntity penaltyEntity, CancellationToken cancellationToken)
    {
        penaltyEntity.Corner = postPenalty.Corner;
        penaltyEntity.Lap = postPenalty.Lap;
        penaltyEntity.Reason = postPenalty.Reason;
        penaltyEntity.Value = new()
        {
            Type = postPenalty.Type,
            Points = postPenalty.Points,
            Positions = postPenalty.Positions,
            Time = postPenalty.Time,
        };
        return await Task.FromResult(penaltyEntity);
    }

    protected virtual async Task<ICollection<AcceptedReviewVoteEntity>> MapToAcceptedVoteList(IEnumerable<VoteModel> voteModels,
        ICollection<AcceptedReviewVoteEntity> voteEntities, CancellationToken cancellationToken)
    {
        // Map votes
        foreach (var voteModel in voteModels)
        {
            var voteEntity = voteEntities
                .FirstOrDefault(x => x.ReviewVoteId == voteModel.Id && voteModel.Id != 0);
            if (voteEntity == null)
            {
                voteEntity = new AcceptedReviewVoteEntity();
                voteEntities.Add(voteEntity);
            }
            await MapToAcceptedReviewVoteEntityAsync(voteModel, voteEntity, cancellationToken);
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

    protected virtual async Task<AcceptedReviewVoteEntity> MapToAcceptedReviewVoteEntityAsync(VoteModel voteModel, AcceptedReviewVoteEntity voteEntity,
        CancellationToken cancellationToken)
    {
        voteEntity.Description = voteModel.Description;
        voteEntity.MemberAtFault = await GetMemberEntityAsync(voteModel.MemberAtFault?.MemberId, cancellationToken);
        voteEntity.TeamAtFault = await GetTeamEntityAsync(voteModel.TeamAtFault?.TeamId, cancellationToken);
        voteEntity.VoteCategory = await GetVoteCategoryEntityAsync(voteEntity.LeagueId, voteModel.VoteCategoryId, cancellationToken);
        return voteEntity;
    }

    protected virtual async Task<ReviewModel?> MapToReviewModel(long reviewId, bool includeComments, CancellationToken cancellationToken)
    {
        var query = dbContext.IncidentReviews
            .Where(x => x.ReviewId == reviewId)
            .Select(MapToReviewModelExpression(includeComments));
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    protected Expression<Func<IncidentReviewEntity, ReviewModel>> MapToReviewModelExpression(bool includeComments) => review => new ReviewModel()
    {
        LeagueId = review.LeagueId,
        ReviewId = review.ReviewId,
        SessionId = review.SessionId,
        SessionNr = review.Session.SessionNr,
        SessionName = review.Session.Name,
        EventId = review.Session.EventId,
        SeasonId = review.Session.Event.Schedule.SeasonId,
        AuthorName = review.AuthorName,
        AuthorUserId = review.AuthorUserId,
        Corner = review.Corner,
        FullDescription = review.FullDescription,
        IncidentKind = review.IncidentKind,
        IncidentNr = review.IncidentNr,
        OnLap = review.OnLap,
        ReviewComments = includeComments ? review.Comments.Select(comment => new ReviewCommentModel()
        {
            CommentId = comment.CommentId,
            AuthorName = comment.AuthorName,
            AuthorUserId = comment.AuthorUserId,
            Date = TreatAsUTCDateTime(comment.Date),
            LeagueId = comment.LeagueId,
            ReviewId = comment.ReviewId.GetValueOrDefault(),
            Text = comment.Text,
            Votes = comment.ReviewCommentVotes.Select(vote => new VoteModel()
            {
                Id = vote.ReviewVoteId,
                VoteCategoryId = vote.VoteCategoryId.GetValueOrDefault(),
                VoteCategoryText = vote.VoteCategory.Text,
                Description = vote.Description,
                MemberAtFault = vote.MemberAtFault == null ? null : new MemberInfoModel()
                {
                    MemberId = vote.MemberAtFault.Id,
                    FirstName = vote.MemberAtFault.Firstname,
                    LastName = vote.MemberAtFault.Lastname,
                },
            }).ToList(),
            CreatedByUserId = comment.CreatedByUserId,
            CreatedByUserName = comment.CreatedByUserName,
            CreatedOn = TreatAsUTCDateTime(comment.CreatedOn),
            LastModifiedByUserId = comment.LastModifiedByUserId,
            LastModifiedByUserName = comment.LastModifiedByUserName,
            LastModifiedOn = TreatAsUTCDateTime(comment.LastModifiedOn),
        }) : Array.Empty<ReviewCommentModel>(),
        InvolvedMembers = review.InvolvedMembers.Select(member => new MemberInfoModel()
        {
            MemberId = member.Id,
            FirstName = member.Firstname,
            LastName = member.Lastname,
        }).ToList(),
        InvolvedTeams = review.InvolvedTeams.Select(team => new TeamInfoModel()
        {
            TeamId = team.TeamId,
            Name = team.Name,
            TeamColor = team.TeamColor,
        }).ToList(),
        ResultText = review.ResultLongText,
        VoteResults = review.AcceptedReviewVotes.Select(vote => new VoteModel()
        {
            Id = vote.ReviewVoteId,
            VoteCategoryId = vote.VoteCategoryId.GetValueOrDefault(),
            VoteCategoryText = vote.VoteCategory.Text,
            Description = vote.Description,
            MemberAtFault = vote.MemberAtFault != null ? new MemberInfoModel()
            {
                MemberId = vote.MemberAtFault.Id,
                FirstName = vote.MemberAtFault.Firstname,
                LastName = vote.MemberAtFault.Lastname,
            } : default,
            TeamAtFault = vote.TeamAtFault != null ? new TeamInfoModel()
            {
                TeamId = vote.TeamAtFault.TeamId,
                Name = vote.TeamAtFault.Name,
                TeamColor = vote.TeamAtFault.TeamColor,
            } : default,
        }).ToList(),
        TimeStamp = review.TimeStamp,
        CreatedByUserId = review.CreatedByUserId,
        CreatedByUserName = review.CreatedByUserName,
        CreatedOn = TreatAsUTCDateTime(review.CreatedOn),
        LastModifiedByUserId = review.LastModifiedByUserId,
        LastModifiedByUserName = review.LastModifiedByUserName,
        LastModifiedOn = TreatAsUTCDateTime(review.LastModifiedOn),
    };

    protected Expression<Func<ReviewPenaltyEntity, PenaltyModel>> MapToReviewPenaltyModelExpression => penalty => new()
    {
        ResultRowId = penalty.ResultRowId,
        ReviewId = penalty.ReviewId,
        ReviewVoteId = penalty.ReviewVoteId,
        Lap = penalty.Review.OnLap,
        Corner = penalty.Review.Corner,
        EventId = penalty.ResultRow.ScoredSessionResult.ScoredEventResult.EventId,
        SessionNr = penalty.ResultRow.ScoredSessionResult.SessionNr,
        SessionName = penalty.ResultRow.ScoredSessionResult.Name,
        Member = penalty.ResultRow.Member == null ? default : new()
        {
            MemberId = penalty.ResultRow.Member.Id,
            FirstName = penalty.ResultRow.Member.Firstname,
            LastName = penalty.ResultRow.Member.Lastname,
        },
        Team = penalty.ResultRow.Team == null ? default : new()
        {
            TeamId = penalty.ResultRow.Team.TeamId,
            Name = penalty.ResultRow.Team.Name,
            TeamColor = penalty.ResultRow.Team.TeamColor,
        },
        Reason = $"{penalty.ReviewVote.VoteCategory.Text} - {penalty.Review.IncidentKind}",
        Type = penalty.Value.Type,
        Points = (int)penalty.Value.Points,
        Time = penalty.Value.Time,
        Positions = penalty.Value.Positions,
    };

    protected async Task<PenaltyModel?> MapToAddPenaltyModel(long addPenaltyId, CancellationToken cancellationToken)
    {
        return await dbContext.AddPenaltys
            .Where(x => x.AddPenaltyId == addPenaltyId)
            .Select(MapToAddPenaltyModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }
    protected Expression<Func<AddPenaltyEntity, PenaltyModel>> MapToAddPenaltyModelExpression => penalty => new()
    {
        ResultRowId = penalty.ScoredResultRowId,
        AddPenaltyId = penalty.AddPenaltyId,
        Lap = penalty.Lap,
        Corner = penalty.Corner,
        Member = penalty.ScoredResultRow.Member == null ? default : new()
        {
            MemberId = penalty.ScoredResultRow.Member.Id,
            FirstName = penalty.ScoredResultRow.Member.Firstname,
            LastName = penalty.ScoredResultRow.Member.Lastname,
        },
        Team = penalty.ScoredResultRow.Team == null ? default : new()
        {
            TeamId = penalty.ScoredResultRow.Team.TeamId,
            Name = penalty.ScoredResultRow.Team.Name,
            TeamColor = penalty.ScoredResultRow.Team.TeamColor,
        },
        EventId = penalty.ScoredResultRow.ScoredSessionResult.ScoredEventResult.EventId,
        SessionNr = penalty.ScoredResultRow.ScoredSessionResult.SessionNr,
        SessionName = penalty.ScoredResultRow.ScoredSessionResult.Name,
        Reason = penalty.Reason,
        Type = penalty.Value.Type,
        Points = (int)penalty.Value.Points,
        Time = penalty.Value.Time,
        Positions = penalty.Value.Positions,
    };
}
