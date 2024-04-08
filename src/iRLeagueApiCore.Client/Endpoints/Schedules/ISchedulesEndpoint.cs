using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Schedules;

public interface ISchedulesEndpoint : IPostGetAllEndpoint<ScheduleModel, PostScheduleModel>, IWithIdEndpoint<IScheduleByIdEndpoint>
{
}
