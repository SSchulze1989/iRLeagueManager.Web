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
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

public interface ILeagueByNameEndpoint : IGetEndpoint<LeagueModel>
{
    string Name { get; }
    ISeasonsEndpoint Seasons();
    ISchedulesEndpoint Schedules();
    IEventsEndpoint Events();
    IResultsEndpoint Results();
    IResultConfigsEndpoint ResultConfigs();
    IPointRulesEndpoint PointRules();
    IProtestsEndpoint Protests();
    IReviewsEndpoint Reviews();
    IReviewCommentsEndpoint ReviewComments();
    ISessionsEndpoint Sessions();
    ILeagueUsersEndpoint Users();
    ITeamsEndpoint Teams();
    IMembersEndpoint Members();
    IVoteCategoriesEndpoint VoteCategories();
    IChampionshipsEndpoint Championships();
    IChampSeasonsEndpoint ChampSeasons();
    IPenaltiesEndpoint Penalties();
    ICustomEndpoint<T> CustomEndpoint<T>(string route);
}
