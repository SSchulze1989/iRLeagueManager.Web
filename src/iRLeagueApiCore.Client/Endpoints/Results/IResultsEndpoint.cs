using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Results;

public interface IResultsEndpoint : IWithIdEndpoint<IResultByIdEndpoint>
{
    public IGetEndpoint<IEnumerable<EventResultModel>> Latest();
}
