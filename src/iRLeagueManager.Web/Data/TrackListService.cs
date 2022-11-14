using iRLeagueApiCore.Common.Models.Tracks;

namespace iRLeagueManager.Web.Data
{
    public class TrackListService
    {
        private readonly LeagueApiService apiService;
        private readonly TimeSpan updateInterval = TimeSpan.FromHours(1);

        private DateTime lastUpdate = DateTime.MinValue;
        private IList<TrackGroupModel> cachedTrackList = Array.Empty<TrackGroupModel>();

        public TrackListService(LeagueApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<IList<TrackGroupModel>> GetTracks()
        {
            if (lastUpdate < (DateTime.UtcNow - updateInterval))
            {
                var request = apiService.Client.Tracks().Get();
                var result = await request;
                if (result.Success && result.Content is not null)
                {
                    lastUpdate = DateTime.UtcNow;
                    cachedTrackList = result.Content.ToList();
                }
            }
            return cachedTrackList;
        }
    }
}
