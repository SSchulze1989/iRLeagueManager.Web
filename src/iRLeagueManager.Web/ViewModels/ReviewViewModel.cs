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
    }
}
