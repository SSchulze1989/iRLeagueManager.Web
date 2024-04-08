using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Results;

public interface IResultConfigsEndpoint : IGetAllEndpoint<ResultConfigModel>, IWithIdEndpoint<IResultConfigByIdEndpoint>
{
}
