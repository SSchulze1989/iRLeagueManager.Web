using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using iRLeagueManager.Web.Shared;
using System.IO.IsolatedStorage;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ReviewViewModel : LeagueViewModelBase<ReviewViewModel, ReviewModel>
{
    public ReviewViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService, new())
    {
    }
    public ReviewViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ReviewModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public long ReviewId => model.ReviewId;
    public long EventId => model.EventId;
    public string SessionName => model.SessionName;
    public int SessionNr => model.SessionNr;
    public long SeasonId => model.SeasonId;
    public string AuthorName => model.AuthorName;
    public string AuthorUserId => model.AuthorUserId;
    public DateTime CreatedOn => model.CreatedOn.GetValueOrDefault();
    public DateTime LastModifiedOn => model.LastModifiedOn.GetValueOrDefault();

    private long? sessionId;
    public long? SessionId { get => sessionId; set => Set(ref sessionId, value); }
    public string IncidentKind { get => model.IncidentKind; set => SetP(model.IncidentKind, value => model.IncidentKind = value, value); }
    public string FullDescription { get => model.FullDescription; set => SetP(model.FullDescription, value => model.FullDescription = value, value); }
    public string OnLap { get => model.OnLap; set => SetP(model.OnLap, value => model.OnLap = value, value); }
    public string Corner { get => model.Corner; set => SetP(model.Corner, value => model.Corner = value, value); }
    public TimeSpan TimeStamp { get => model.TimeStamp; set => SetP(model.TimeStamp, value => model.TimeStamp = value, value); }
    public string IncidentNr { get => model.IncidentNr; set => SetP(model.IncidentNr, value => model.IncidentNr = value, value); }
    public string ResultText { get => model.ResultText; set => SetP(model.ResultText, value => model.ResultText = value, value); }
    public IEnumerable<MemberInfoModel> InvolvedMembers { get => model.InvolvedMembers; set => SetP(model.InvolvedMembers, value => model.InvolvedMembers = value.ToList(), value); }

    private IList<MemberInfoModel> involvedMembers = new List<MemberInfoModel>();
    public IList<MemberInfoModel> InvolvedMembers { get => involvedMembers; set => Set(ref involvedMembers, value); }

    private IList<TeamInfoModel> involvedTeams = new List<TeamInfoModel>();
    public IList<TeamInfoModel> InvolvedTeams { get => involvedTeams; set => Set(ref involvedTeams, value); }

    private ObservableCollection<ReviewCommentViewModel> comments = new();
    public ReadOnlyObservableCollection<ReviewCommentViewModel> Comments => new(comments);

    private ObservableCollection<VoteViewModel> votes = new();
    public ObservableCollection<VoteViewModel> Votes { get => votes; set => Set(ref votes, value); }

    private ReviewStatus status;
    public ReviewStatus Status { get => status; set => Set(ref status, value); }

    public IEnumerable<CountedVote> CountedVotes => GetCountedVotes();

    public void AddVote(VoteViewModel vote)
    {
        votes.Add(vote);
        model.VoteResults.Add(vote.GetModel());
        UpdateReviewStatus();
    }

    public void AddVote(VoteModel vote)
    {
        model.VoteResults.Add(vote);
        votes.Add(new(LoggerFactory, ApiService, vote));
        UpdateReviewStatus();
    }

    public void RemoveVote(VoteViewModel vote)
    {
        votes.Remove(vote);
        model.VoteResults.Remove(vote.GetModel());
        UpdateReviewStatus();
    }

    /// <summary>
    /// Add a comment to the review
    /// </summary>
    /// <param name="postComment"></param>
    /// <returns></returns>
    public async Task<StatusResult> AddComment(PostReviewCommentModel postComment, CancellationToken cancellationToken)
    {
        // Get the endpoint
        var reviewEndpoint = GetReviewEndpoint();
        if (reviewEndpoint == null)
        {
            return LeagueNullResult();
        }

        // Upload comment
        var result = await reviewEndpoint.ReviewComments().Post(postComment);
        if (result.Success && result.Content is not null)
        {
            // Update comment list
            model.ReviewComments = model.ReviewComments.Concat(new[] { result.Content });
            RefreshCommentList();
        }

        return result.ToStatusResult();
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="commentId"></param>
    /// <returns></returns>
    public async Task<StatusResult> RemoveComment(ReviewCommentViewModel comment, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague is null)
        {
            return LeagueNullResult();
        }

        var commentEndpoint = ApiService.CurrentLeague
            .ReviewComments()
            .WithId(comment.CommentId);

        var result = await commentEndpoint.Delete(cancellationToken);
        if (result.Success)
        {
            model.ReviewComments = model.ReviewComments.Except(new[] { comment.GetModel() });
            RefreshCommentList();
        }

        return result.ToStatusResult();
    }

    /// <summary>
    /// Get endpoint for this review by the current <see cref="ReviewId"/> value
    /// </summary>
    /// <returns></returns>
    private IReviewByIdEndpoint? GetReviewEndpoint()
    {
        if (ReviewId == 0)
        {
            return null;
        }
        return ApiService.CurrentLeague?.Reviews().WithId(ReviewId);
    }

    /// <summary>
    /// Refresh comment list with the current comment models
    /// </summary>
    private void RefreshCommentList()
    {
        // Add comments from to list that are not already in there
        foreach (var comment in model.ReviewComments)
        {
            if (comments.Any(x => x.GetModel() == comment) == false)
            {
                var commentViewModel = new ReviewCommentViewModel(LoggerFactory, ApiService, comment);
                comments.Add(commentViewModel);
            }
        }
        // Remove comments that are no longer in the model
        foreach (var commentViewModel in Comments.ToArray())
        {
            if (model.ReviewComments.Any(x => x == commentViewModel.GetModel()) == false)
            {
                comments.Remove(commentViewModel);
            }
        }
        UpdateReviewStatus();
    }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (model == null)
        {
            return StatusResult.SuccessResult();
        }
        if (ApiService.CurrentLeague == null || ReviewId == 0)
        {
            return LeagueNullResult();
        }

        UpdateInvolved();

        try
        {
            Loading = true;
            // if session was changed -> move review
            if (SessionId != null && SessionId != model.SessionId)
            {
                var moveRequest = ApiService.CurrentLeague
                    .Reviews()
                    .WithId(ReviewId)
                    .MoveToSession(SessionId.Value);
                var moveResult = await moveRequest.Post(cancellationToken);
                if (moveResult.Success == false)
                {
                    return moveResult.ToStatusResult();
                }
            }

            var request = ApiService.CurrentLeague
                .Reviews()
                .WithId(ReviewId)
                .Put(model, cancellationToken);
            var result = await request;
            if (result.Success == false || result.Content is null)
            {
                return result.ToStatusResult();
            }
            SetModel(result.Content);
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private void RefreshVoteList()
    {
        Votes = new ObservableCollection<VoteViewModel>(model.VoteResults.Select(x => new VoteViewModel(LoggerFactory, ApiService, x)));
    }

    private IEnumerable<CountedVote> GetCountedVotes()
    {
        List<CountedVote> countedVotes = new();
        foreach (var vote in Comments.SelectMany(x => x.Votes))
        {
            CountedVote? countedVote = countedVotes
                .FirstOrDefault(x => CompareVotes(vote, x.Vote));
            if (countedVote == null)
            {
                countedVote = new(vote);
                countedVotes.Add(countedVote);
            }
            countedVote.Count++;
        }
        return countedVotes;
    }

    public void UpdateReviewStatus()
    {
        if (IsClosed())
        {
            Status = ReviewStatus.Closed;
            return;
        }
        if (IsVoteConflicted())
        {
            Status = ReviewStatus.OpenConflicted;
            return;
        }
        if (HasMajorityVote(2))
        {
            Status = ReviewStatus.OpenEnoughVotes;
            return;
        }

        Status = ReviewStatus.Open;
    }

    private void UpdateInvolved()
    {
        model.InvolvedMembers = InvolvedMembers;
        model.InvolvedTeams = InvolvedTeams;
    }

    private bool IsClosed()
    {
        return Votes.Count > 0;
    }

    private bool IsVoteConflicted()
    {
        var votes = Comments
            .Select(x => x.Votes.Select(y => y.GetModel()))
            .ToList();
        bool hasConflict = false;
        for (int i=0; i<votes.Count-1; i++)
        {
            hasConflict |= !CompareVotes(votes[i], votes[i + 1]);
        }
        return hasConflict;
    }

    private bool CompareVotes(IEnumerable<VoteModel> first, IEnumerable<VoteModel> second)
    {
        return first.OrderBy(x => x.MemberAtFault?.MemberId).Zip(second.OrderBy(x => x.MemberAtFault?.MemberId))
            .All(x => x.First.VoteCategoryId == x.Second.VoteCategoryId);
    }

    private bool HasMajorityVote(int minVoteCount)
    {
        return Comments.Count(x => x.Votes.Any()) >= minVoteCount;
    }

    private static bool CompareVotes(VoteViewModel vote1, VoteViewModel vote2)
    {
        return vote1.MemberAtFault?.MemberId == vote2.MemberAtFault?.MemberId && vote1.VoteCategoryId == vote2.VoteCategoryId;
    }

    public async Task<StatusResult> AddToSessionAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        if (model == null)
        {
            return StatusResult.SuccessResult();
        }
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        UpdateInvolved();

        try
        {
            Loading = true;
            var endpoint = ApiService.CurrentLeague.Sessions()
                .WithId(sessionId)
                .Reviews();
            var result = await endpoint.Post(model, cancellationToken);
            if (result.Success == false)
            {
                return result.ToStatusResult();
            }
            SetModel(model);
            return StatusResult.SuccessResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<bool> DeleteAsync(CancellationToken cancellationToken = default)
    {
        if (model == null)
        {
            return true;
        }
        if (ApiService.CurrentLeague == null)
        {
            return false;
        }

        try
        {
            Loading = true;
            var request = ApiService.CurrentLeague.Reviews()
                .WithId(ReviewId)
                .Delete(cancellationToken);
            var result = await request;
            if (result.Success == false)
            {
                result.EnsureSuccess();
                return false;
            }
            return true;
        }
        finally
        {
            Loading = false;
        }
    }

    public override void SetModel(ReviewModel model)
    {
        base.SetModel(model);
        InvolvedMembers = model.InvolvedMembers.ToList();
        InvolvedTeams = model.InvolvedTeams.ToList();
        RefreshCommentList();
        RefreshVoteList();
        UpdateReviewStatus();
        SessionId = model.SessionId == 0 ? null : model.SessionId;
    }
}
