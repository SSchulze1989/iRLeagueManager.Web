using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Results;

public interface IResultByIdEndpoint : IGetEndpoint<EventResultModel>
{
    IGetAllEndpoint<PenaltyModel> Penalties();
}
