using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Common.Models.Members;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ReviewViewModel : LeagueViewModelBase<ReviewViewModel, ReviewModel>
    {
        public ReviewViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ReviewModel model) :
            base(loggerFactory, apiService, model)
        {
        }

        public long ReviewId => model.ReviewId;
        public long EventId => model.EventId;
        public long SeasonId => model.SeasonId;
        public long SessionId => model.SessionId;
        public string AuthorName => model.AuthorName;
        public string AuthorUserId => model.AuthorUserId;
        public DateTime CreatedOn => model.CreatedOn.GetValueOrDefault();
        public DateTime LastModifiedON => model.LastModifiedOn.GetValueOrDefault();

        public string IncidentKind { get => model.IncidentKind; set => SetP(model.IncidentKind, value => model.IncidentKind = value, value); }
        public string FullDescription { get => model.FullDescription; set => SetP(model.FullDescription, value => model.FullDescription = value, value); }
        public string OnLap { get => model.OnLap; set => SetP(model.OnLap, value => model.OnLap = value, value); }
        public string Corner { get => model.Corner; set => SetP(model.Corner, value => model.Corner = value, value); }
        public TimeSpan TimeStamp { get => model.TimeStamp; set => SetP(model.TimeStamp, value => model.TimeStamp = value, value); }
        public string IncidentNr { get => model.IncidentNr; set => SetP(model.IncidentNr, value => model.IncidentNr = value, value); }

        private ObservableCollection<MemberInfoModel> involvedMembers;
        public ObservableCollection<MemberInfoModel> InvolvedMembers { get => involvedMembers; set => Set(ref involvedMembers, value); }

        private ObservableCollection<ReviewCommentViewModel> comments = new();
        public ObservableCollection<ReviewCommentViewModel> Comments { get => comments; set => Set(ref comments, value); }

        /// <summary>
        /// Add one or more members to the <see cref="InvolvedMembers"/> selection"/>
        /// </summary>
        /// <param name="selection"></param>
        public void AddMemberSelection(IEnumerable<MemberInfoModel> selection)
        {
            foreach(var member in selection)
            {
                if (InvolvedMembers.Contains(member))
                {
                    continue;
                }
                if (member.MemberId == 0)
                {
                    continue;
                }
                if (InvolvedMembers.Any(x => x.MemberId == member.MemberId))
                {
                    continue;
                }
                InvolvedMembers.Add(member);
            }
        }

        /// <summary>
        /// Remove one or more members from the <see cref="InvolvedMembers"/> selection"/>
        /// </summary>
        /// <param name="selection"></param>
        public void RemoveMemberSelection(IEnumerable<MemberInfoModel> selection)
        {
            foreach(var member in selection)
            {
                if (InvolvedMembers.Contains(member))
                {
                    InvolvedMembers.Remove(member);
                    continue;
                }
                if (member.MemberId == 0)
                {
                    continue;
                }
                var involvedMember = InvolvedMembers.FirstOrDefault(x => x.MemberId == member.MemberId);
                if (involvedMember != null)
                {
                    InvolvedMembers.Remove(involvedMember);
                    continue;
                }
            }
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

        /// <summary>
        /// Refresh comment list with the current comment models
        /// </summary>
        private void RefreshCommentList()
        {
            // Add comments from to list that are not already in there
            foreach(var comment in model.ReviewComments)
            {
                if (Comments.Any(x => x.GetModel() == comment) == false)
                {
                    Comments.Add(new ReviewCommentViewModel(LoggerFactory, ApiService, comment));
                }
            }
            // Remove comments that are no longer in the model
            foreach(var commentViewModel in Comments)
            {
                if (model.ReviewComments.Any(x => x == commentViewModel.GetModel()) == false)
                {
                    Comments.Remove(commentViewModel);
                }
            }
        }
    }
}
