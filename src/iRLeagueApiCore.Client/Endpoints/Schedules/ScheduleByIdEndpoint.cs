using iRLeagueApiCore.Client.Endpoints.Sessions;
using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Schedules;

internal class ScheduleByIdEndpoint : UpdateEndpoint<ScheduleModel, PutScheduleModel>, IScheduleByIdEndpoint
{
    public ScheduleByIdEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder, long scheduleId) :
        base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddParameter(scheduleId);
    }

    public IPostGetAllEndpoint<EventModel, PostEventModel> Events()
    {
        return new EventsEndpoint(HttpClientWrapper, RouteBuilder);
    }
}
