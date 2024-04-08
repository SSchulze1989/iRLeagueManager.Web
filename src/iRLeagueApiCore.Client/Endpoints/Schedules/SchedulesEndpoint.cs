using iRLeagueApiCore.Client.Http;
using iRLeagueApiCore.Client.QueryBuilder;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Schedules;

internal class SchedulesEndpoint : PostGetAllEndpoint<ScheduleModel, PostScheduleModel>, ISchedulesEndpoint
{
    public SchedulesEndpoint(HttpClientWrapper httpClientWrapper, RouteBuilder routeBuilder) : base(httpClientWrapper, routeBuilder)
    {
        RouteBuilder.AddEndpoint("Schedules");
    }

    IScheduleByIdEndpoint IWithIdEndpoint<IScheduleByIdEndpoint, long>.WithId(long id)
    {
        return new ScheduleByIdEndpoint(HttpClientWrapper, RouteBuilder, id);
    }
}
