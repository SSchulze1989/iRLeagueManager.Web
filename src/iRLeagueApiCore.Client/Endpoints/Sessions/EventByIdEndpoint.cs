using iRLeagueApiCore.Client.Endpoints.Cars;
using iRLeagueApiCore.Client.Endpoints.Members;
using iRLeagueApiCore.Client.Endpoints.Protests;
using iRLeagueApiCore.Client.Endpoints.Results;
using iRLeagueApiCore.Client.Endpoints.Reviews;
using iRLeagueApiCore.Client.Endpoints.Standings;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Results;
using iRLeagueApiCore.Common.Models.Reviews;
using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueApiCore.Client.Endpoints.Sessions;

internal class EventByIdEndpoint : UpdateEndpoint<EventModel, PutEventModel>, IEventByIdEndpoint
{
    public EventByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long EventId) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(EventId);
    }

    public IGetAllEndpoint<MemberModel> Members()
    {
        return new MembersEndpoint(HttpClientWrapper, RouteBuilder);
    }

    public IGetEndpoint<CarListModel> Cars()
    {
        return new CarsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IGetAllEndpoint<ProtestModel> IEventByIdEndpoint.Protests()
    {
        return new ProtestsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IEventResultsEndpoint IEventByIdEndpoint.Results()
    {
        return new ResultsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IGetAllEndpoint<ReviewModel> IEventByIdEndpoint.Reviews()
    {
        return new ReviewsEndpoint(HttpClientWrapper, RouteBuilder);
    }

    IGetAllEndpoint<StandingsModel> IEventByIdEndpoint.Standings()
    {
        return new StandingsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
