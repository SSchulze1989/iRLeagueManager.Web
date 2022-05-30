using iRLeagueManager.Web.ViewModels;

namespace iRLeagueManager.Web
{
    public static class Services
    {
        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddScoped<LeaguesViewModel>();
            services.AddScoped<SeasonsViewModel>();
            services.AddScoped<SchedulesPageViewModel>();
            services.AddScoped<ScheduleViewModel>();
            services.AddScoped<SessionViewModel>();
            services.AddScoped<ResultsPageViewModel>();
        }
    }
}
