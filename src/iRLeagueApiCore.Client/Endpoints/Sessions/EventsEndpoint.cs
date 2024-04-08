using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Sessions;

internal class EventsEndpoint : PostGetAllEndpoint<EventModel, PostEventModel>, IEventsEndpoint,
    IPostEndpoint<EventModel, PostEventModel>, IGetAllEndpoint<EventModel>, IWithIdEndpoint<IEventByIdEndpoint>
{
    public EventsEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Events");
    }

    IEventByIdEndpoint IWithIdEndpoint<IEventByIdEndpoint, long>.WithId(long id)
    {
        return new EventByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
