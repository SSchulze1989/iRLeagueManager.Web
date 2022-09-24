using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ReviewCommentViewModel : LeagueViewModelBase<ReviewCommentViewModel, ReviewCommentModel>
    {
        public ReviewCommentViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ReviewCommentModel model) :
            base(loggerFactory, apiService, model)
        {
        }

        public long CommentId => model.CommentId;
        public long ReviewId => model.ReviewId;
        public DateTime Date => model.Date.GetValueOrDefault();
        public string AuthorName => model.AuthorName;
        public string AuthorUserId => model.AuthorUserId;
        public DateTime CreatedOn => model.CreatedOn.GetValueOrDefault();
        public DateTime LastModifiedOn => model.LastModifiedOn.GetValueOrDefault();

        public string Text { get => model.Text; set => SetP(model.Text, value => model.Text = value, value); }
        
        private ObservableCollection<VoteViewModel> votes = new();
        public ObservableCollection<VoteViewModel> Votes { get => votes; set => Set(ref votes, value); }

        public void AddVote(VoteViewModel vote)
        {
            votes.Add(vote);
            model.Votes.Add(vote.GetModel());
        }

        public void RemoveVote(VoteViewModel vote)
        {
            votes.Remove(vote);
            model.Votes.Remove(vote.GetModel());
        }

        private void RefreshVoteList()
        {
            Votes = new ObservableCollection<VoteViewModel>(model.Votes.Select(x => new VoteViewModel(LoggerFactory, ApiService, x)));
        }

        public override void SetModel(ReviewCommentModel model)
        {
            base.SetModel(model);
            RefreshVoteList();
        }
    }
}