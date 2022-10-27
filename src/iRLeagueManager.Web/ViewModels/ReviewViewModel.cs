using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Common.Models.Members;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ReviewViewModel : LeagueViewModelBase<ReviewViewModel, ReviewModel>
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

        private IList<MemberInfoModel> involvedMembers = new List<MemberInfoModel>();
        public IList<MemberInfoModel> InvolvedMembers { get => involvedMembers; set => Set(ref involvedMembers, value); }

        private ObservableCollection<ReviewCommentViewModel> comments = new();
        public ObservableCollection<ReviewCommentViewModel> Comments { get => comments; set => Set(ref comments, value); }

        private ObservableCollection<VoteViewModel> votes = new();
        public ObservableCollection<VoteViewModel> Votes { get => votes; set => Set(ref votes, value); }

        public IEnumerable<CountedVote> CountedVotes => GetCountedVotes();

        public void AddVote(VoteViewModel vote)
        {
            votes.Add(vote);
            model.VoteResults.Add(vote.GetModel());
        }

        public void RemoveVote(VoteViewModel vote)
        {
            votes.Remove(vote);
            model.VoteResults.Remove(vote.GetModel());
        }

        /// <summary>
        /// Add one or more members to the <see cref="InvolvedMembers"/> selection"/>
        /// </summary>
        /// <param name="selection"></param>
        public void AddMemberSelection(IEnumerable<MemberInfoModel> selection)
        {
            foreach (var member in selection)
            {
                if (model.InvolvedMembers.Contains(member))
                {
                    continue;
                }
                if (member.MemberId == 0)
                {
                    continue;
                }
                if (model.InvolvedMembers.Any(x => x.MemberId == member.MemberId))
                {
                    continue;
                }
                model.InvolvedMembers.Add(member);
            }
            RefreshMemberList();
        }

        /// <summary>
        /// Remove one or more members from the <see cref="InvolvedMembers"/> selection"/>
        /// </summary>
        /// <param name="selection"></param>
        public void RemoveMemberSelection(IEnumerable<MemberInfoModel> selection)
        {
            foreach (var member in selection)
            {
                if (model.InvolvedMembers.Contains(member))
                {
                    model.InvolvedMembers.Remove(member);
                    continue;
                }
                if (member.MemberId == 0)
                {
                    continue;
                }
                var involvedMember = model.InvolvedMembers.FirstOrDefault(x => x.MemberId == member.MemberId);
                if (involvedMember != null)
                {
                    model.InvolvedMembers.Remove(involvedMember);
                    continue;
                }
            }
            RefreshMemberList();
        }

        /// <summary>
        /// Add a comment to the review
        /// </summary>
        /// <param name="postComment"></param>
        /// <returns></returns>
        public async Task<bool> AddComment(PostReviewCommentModel postComment)
        {
            // Get the endpoint
            var reviewEndpoint = GetReviewEndpoint();
            if (reviewEndpoint == null)
            {
                return false;
            }

            // Upload comment
            var result = await reviewEndpoint.ReviewComments().Post(postComment);
            if (result.Success == false)
            {
                return false;
            }

            // Update comment list
            RefreshCommentList();

            return true;
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveComment(long commentId)
        {
            var commentEndpoint = ApiService.CurrentLeague?
                .ReviewComments()
                .WithId(commentId);
            if (commentEndpoint == null)
            {
                return false;
            }

            var result = await commentEndpoint.Delete();
            return result.Success;
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

        private void UpdateModelMemberList()
        {
            foreach(var member in InvolvedMembers)
            {
                if (model.InvolvedMembers.Any(x => x.MemberId == member.MemberId) == false)
                {
                    model.InvolvedMembers.Add(member);
                }
            }
            foreach(var member in model.InvolvedMembers.ToArray())
            {
                if (InvolvedMembers.Any(x => x.MemberId == member.MemberId) == false)
                {
                    model.InvolvedMembers.Remove(member);
                }
            }
        }

        private void RefreshMemberList()
        {
            // Add comments from to list that are not already in there
            foreach (var member in model.InvolvedMembers)
            {
                if (InvolvedMembers.Any(x => x.MemberId == member.MemberId) == false)
                {
                    InvolvedMembers.Add(member);
                }
            }
            // Remove comments that are no longer in the model
            foreach (var member in InvolvedMembers.ToArray())
            {
                if (model.InvolvedMembers.Any(x => x.MemberId == member.MemberId) == false)
                {
                    InvolvedMembers.Remove(member);
                }
            }
        }

        /// <summary>
        /// Refresh comment list with the current comment models
        /// </summary>
        private void RefreshCommentList()
        {
            // Add comments from to list that are not already in there
            foreach (var comment in model.ReviewComments)
            {
                if (Comments.Any(x => x.GetModel() == comment) == false)
                {
                    Comments.Add(new ReviewCommentViewModel(LoggerFactory, ApiService, comment));
                }
            }
            // Remove comments that are no longer in the model
            foreach (var commentViewModel in Comments.ToArray())
            {
                if (model.ReviewComments.Any(x => x == commentViewModel.GetModel()) == false)
                {
                    Comments.Remove(commentViewModel);
                }
            }
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

            UpdateModelMemberList();

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
                if (result.Success == false)
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
            foreach(var vote in Comments.SelectMany(x => x.Votes))
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

        private bool CompareVotes(VoteViewModel vote1, VoteViewModel vote2)
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

            UpdateModelMemberList();

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
            RefreshMemberList();
            RefreshCommentList();
            RefreshVoteList();
            SessionId = model.SessionId;
        }
    }
}
