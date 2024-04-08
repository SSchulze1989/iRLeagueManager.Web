using iRLeagueApiCore.Client.Endpoints.Championships;
using iRLeagueApiCore.Client.Endpoints.Members;
using iRLeagueApiCore.Client.Endpoints.Penalties;
using iRLeagueApiCore.Client.Endpoints.Protests;
using iRLeagueApiCore.Client.Endpoints.Results;
using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Client.Endpoints.Schedules;
using iRLeagueApiCore.Client.Endpoints.Scorings;
using iRLeagueApiCore.Client.Endpoints.Seasons;
using iRLeagueApiCore.Client.Endpoints.Sessions;
using iRLeagueApiCore.Client.Endpoints.Teams;
using iRLeagueApiCore.Client.Endpoints.Users;
using iRLeagueApiCore.Client.Endpoints.VoteCategories;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

internal sealed class LeagueByNameEndpoint : GetEndpoint<LeagueModel>, ILeagueByNameEndpoint
{
    public string Name { get; }

    public LeagueByNameEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, string leagueName) :
        base(httpClientWrapper, routeBuilder)
    {
        Name = leagueName;
        RouteBuilder.AddParameter(leagueName);
    }

    ISchedulesEndpoint ILeagueByNameEndpoint.Schedules()
    {
        return new SchedulesEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ISeasonsEndpoint ILeagueByNameEndpoint.Seasons()
    {
        return new SeasonsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IEventsEndpoint ILeagueByNameEndpoint.Events()
    {
        return new EventsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IResultConfigsEndpoint ILeagueByNameEndpoint.ResultConfigs()
    {
        return new ResultConfigsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IPointRulesEndpoint ILeagueByNameEndpoint.PointRules()
    {
        return new PointRulesEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IReviewsEndpoint ILeagueByNameEndpoint.Reviews()
    {
        return new ReviewsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IReviewCommentsEndpoint ILeagueByNameEndpoint.ReviewComments()
    {
        return new ReviewCommentsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ISessionsEndpoint ILeagueByNameEndpoint.Sessions()
    {
        return new SessionsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ILeagueUsersEndpoint ILeagueByNameEndpoint.Users()
    {
        return new UsersEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IVoteCategoriesEndpoint ILeagueByNameEndpoint.VoteCategories()
    {
        return new VoteCategoriesEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ITeamsEndpoint ILeagueByNameEndpoint.Teams()
    {
        return new TeamsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IMembersEndpoint ILeagueByNameEndpoint.Members()
    {
        return new MembersEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IProtestsEndpoint ILeagueByNameEndpoint.Protests()
    {
        return new ProtestsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IChampionshipsEndpoint ILeagueByNameEndpoint.Championships()
    {
        return new ChampionshipsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IChampSeasonsEndpoint ILeagueByNameEndpoint.ChampSeasons()
    {
        return new ChampSeasonsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IPenaltiesEndpoint ILeagueByNameEndpoint.Penalties()
    {
        return new PenaltiesEndpoint(HttpClientWrapper, RouteBuilder);
    }

    ICustomEndpoint<T> ILeagueByNameEndpoint.CustomEndpoint<T>(string route)
    {
        return new CustomEndpoint<T>(HttpClientWrapper, RouteBuilder, route);
    }

    IResultsEndpoint ILeagueByNameEndpoint.Results()
    {
        return new ResultsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
