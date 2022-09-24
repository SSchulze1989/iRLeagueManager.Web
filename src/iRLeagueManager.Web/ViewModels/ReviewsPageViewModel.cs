using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class ReviewsPageViewModel : LeagueViewModelBase<ReviewsPageViewModel>
    {
        public ReviewsPageViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
        }

        private ObservableCollection<ReviewViewModel> reviews = new();
        public ObservableCollection<ReviewViewModel> Reviews { get => reviews; set => Set(ref reviews, value); }

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
                if (result.Success == false)
                {
                    return;
                }

                var reviewModels = result.Content;
                Reviews = new(reviewModels.Select(x => new ReviewViewModel(LoggerFactory, ApiService, x)));
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
