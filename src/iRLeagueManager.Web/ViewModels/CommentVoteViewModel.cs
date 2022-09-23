using iRLeagueApiCore.Common.Models.Members;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class CommentVoteViewModel : LeagueViewModelBase<CommentVoteViewModel, CommentVoteModel>
    {
        public CommentVoteViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, CommentVoteModel model) : 
            base(loggerFactory, apiService, model)
        {
        }

        public long Id => model.Id;
        public long VoteCategoryId { get => model.VoteCategoryId; set => SetP(model.VoteCategoryId, value => model.VoteCategoryId = value, value); }
        public string Description { get => model.Description; set => SetP(model.Description, value => model.Description = value, value); }
        public MemberInfoModel MemberAtFault { get => model.MemberAtFault; set => SetP(model.MemberAtFault, value => model.MemberAtFault = value, value); }
    }
}