using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace iRLeagueManager.Web
{
    public static class Services
    {
        public static IServiceCollection AddTrackList(this IServiceCollection services)
        {
            services.TryAddScoped<TrackListService>();
            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.TryAddScoped<LeaguesViewModel>();
            services.TryAddScoped<SeasonsViewModel>();
            services.TryAddScoped<SeasonViewModel>();
            services.TryAddScoped<SchedulesPageViewModel>();
            services.TryAddScoped<ScheduleViewModel>();
            services.TryAddTransient<EventViewModel>();
            services.TryAddTransient<ReviewViewModel>();
            services.TryAddTransient<ReviewCommentViewModel>();
            services.TryAddScoped<ResultsPageViewModel>();
            services.TryAddScoped<ScoringsViewModel>();
            services.TryAddScoped<ScoringViewModel>();
            services.TryAddScoped<ResultConfigSettingsViewModel>();
            services.TryAddScoped<ReviewsPageViewModel>();
            services.TryAddScoped<EventListViewModel>();
            services.TryAddScoped<LeagueUsersViewModel>();
            return services;
        }
    }
}
