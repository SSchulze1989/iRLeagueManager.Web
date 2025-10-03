using Ganss.Xss;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Data.CsvExporter;
using iRLeagueManager.Web.Shared;
using iRLeagueManager.Web.ViewModels;
using Markdig;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace iRLeagueManager.Web;

public static class Services
{
    public static IServiceCollection AddMarkdown(this IServiceCollection services)
    {
        services.TryAddScoped(config => 
            new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseBootstrap()
                .Build());
        services.TryAddScoped(config =>
            new HtmlSanitizer());
        return services;
    }

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
        services.TryAddTransient<LeagueViewModel>();
        services.TryAddTransient<EventViewModel>();
        services.TryAddTransient<ReviewViewModel>();
        services.TryAddTransient<ReviewCommentViewModel>();
        services.TryAddTransient<ResultConfigViewModel>();
        services.TryAddTransient<VoteCategoryViewModel>();
        services.TryAddTransient<PointRuleViewModel>();
        services.TryAddTransient<SortOptionsViewModel>();
        services.TryAddTransient<ResultFiltersViewModel>();
        services.TryAddTransient<TeamViewModel>();
        services.TryAddTransient<ChampionshipViewModel>();
        services.TryAddTransient<ChampSeasonViewModel>();
        services.TryAddTransient<UserViewModel>();
        services.TryAddTransient<AddPenaltyViewModel>();
        services.TryAddTransient<AutoPenaltiesCollectionViewModel>();
        services.TryAddTransient<ResultFilterViewModel>();
        services.TryAddTransient<FilterConditionViewModel>();
        services.TryAddTransient<MemberViewModel>();
        services.TryAddTransient<RosterViewModel>();
        services.TryAddScoped<ResultsPageViewModel>();
        services.TryAddScoped<EditResultViewModel>();
        services.TryAddScoped<ResultSettingsViewModel>();
        services.TryAddScoped<ReviewsPageViewModel>();
        services.TryAddScoped<EventListViewModel>();
        services.TryAddScoped<LeagueUsersViewModel>();
        services.TryAddScoped<StandingsPageViewModel>();
        services.TryAddScoped<ResultSettingsViewModel>();
        services.TryAddScoped<ReviewSettingsViewModel>();
        services.TryAddScoped<TeamsViewModel>();
        services.TryAddScoped<BreadCrumbService>();
        services.TryAddScoped<MemberServiceViewModel>();
        return services;
    }

    public static IServiceCollection AddExporters(this IServiceCollection services)
    {
        var defaultCsvGenerator = new DefaultCsvGenerator();
        services.AddSingleton<IMemberListCsvGenerator>(defaultCsvGenerator);
        services.AddSingleton<ITeamListCsvGenerator>(defaultCsvGenerator);
        services.AddSingleton<IStandingsCsvGenerator>(defaultCsvGenerator);
        services.AddSingleton<IResultsCsvGenerator>(defaultCsvGenerator);
        services.AddSingleton<IGridCsvExporter>(defaultCsvGenerator);
        services.AddSingleton<IRosterListCsvExporter>(defaultCsvGenerator);
        var sdkCsvGenerator = new SdkGamingCsvGenerator();
        services.AddSingleton<IMemberListCsvGenerator>(sdkCsvGenerator);
        services.AddSingleton<ITeamListCsvGenerator>(sdkCsvGenerator);
        services.AddSingleton<IStandingsCsvGenerator>(sdkCsvGenerator);
        services.AddSingleton<IGridCsvExporter>(sdkCsvGenerator);
        services.AddSingleton<IRosterListCsvExporter>(sdkCsvGenerator);
        return services;
    }
}
