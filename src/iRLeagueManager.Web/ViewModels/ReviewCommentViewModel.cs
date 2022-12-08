using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ReviewCommentViewModel : LeagueViewModelBase<ReviewCommentViewModel, ReviewCommentModel>
    {
        public ReviewCommentViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
            base(loggerFactory, apiService, new())
        {
        }
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

        public void AddVote(VoteModel vote)
        {
            model.Votes.Add(vote);
            votes.Add(new VoteViewModel(LoggerFactory, ApiService, vote));
        }

        public void RemoveVote(VoteViewModel vote)
        {
            votes.Remove(vote);
            model.Votes.Remove(vote.GetModel());
        }

        private void RefreshVoteList()
        {
            if (model.Votes == null)
            {
                return;
            }
            Votes = new ObservableCollection<VoteViewModel>(model.Votes.Select(x => new VoteViewModel(LoggerFactory, ApiService, x)));
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
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
                var request = ApiService.CurrentLeague.ReviewComments()
                    .WithId(CommentId)
                    .Put(model);
                var result = await request;
                if (result.Success == false || result.Content is null)
                {
                    result.EnsureSuccess();
                    return false;
                }
                SetModel(result.Content);
                return true;
            }
            finally
            {
                Loading = false;
            }
        }

        public override void SetModel(ReviewCommentModel model)
        {
            base.SetModel(model);
            RefreshVoteList();
        }
    }
}