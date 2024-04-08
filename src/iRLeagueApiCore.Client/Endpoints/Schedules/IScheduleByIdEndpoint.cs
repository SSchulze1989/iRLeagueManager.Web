using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Schedules;

public interface IScheduleByIdEndpoint : IUpdateEndpoint<ScheduleModel, PutScheduleModel>
{
    IPostGetAllEndpoint<EventModel, PostEventModel> Events();
}
