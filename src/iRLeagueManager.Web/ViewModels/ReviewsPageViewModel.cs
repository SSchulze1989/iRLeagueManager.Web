using iRLeagueApiCore.Common.Models.Members;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class ReviewsPageViewModel : LeagueViewModelBase<ReviewsPageViewModel>
{
    public ReviewsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        base(loggerFactory, apiService)
    {
    }

    private ObservableCollection<ReviewViewModel> reviews = new();
    public ObservableCollection<ReviewViewModel> Reviews { get => reviews; set => Set(ref reviews, value); }

    private IEnumerable<MemberInfoModel> eventMembers = Array.Empty<MemberInfoModel>();
    public IEnumerable<MemberInfoModel> EventMembers { get => eventMembers; set => Set(ref eventMembers, value); }

    public async Task LoadFromEventAsync(long eventId, CancellationToken cancellationToken = default)
    {
        if (ApiService.CurrentLeague == null)
        {
            return;
        }

        try
        {
            Loading = true;
            var eventEndpoint = ApiService.CurrentLeague
                .Events()
                .WithId(eventId);
            if (ApiService.CurrentSeason == null)
            {
                var @event = (await eventEndpoint.Get(cancellationToken)).EnsureSuccess();
                if (@event == null)
                {
                    return;
                }
                await ApiService.SetCurrentSeasonAsync(ApiService.CurrentLeague.Name, @event.SeasonId);
            }

            var reviewsEndpoint = eventEndpoint
                .Reviews();
            var result = await reviewsEndpoint.Get(cancellationToken);
            if (result.Success == false || result.Content is null)
            {
                return;
            }

            var reviewModels = result.Content;
            Reviews = new(reviewModels.Select(x => new ReviewViewModel(LoggerFactory, ApiService, x)));

            var membersEndpoint = eventEndpoint
                .Members();
            var membersResult = await membersEndpoint.Get(cancellationToken);
            if (membersResult.Success == false || membersResult.Content is null)
            {
                return;
            }
            EventMembers = membersResult.Content;
        }
        finally
        {
            Loading = false;
        }
    }
}
