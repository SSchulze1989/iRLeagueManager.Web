using iRLeagueApiCore.Client.ResultsParsing;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Results;

public interface IEventResultsEndpoint : IGetAllEndpoint<EventResultModel>, IDeleteEndpoint
{
    IPostEndpoint<bool, ParseSimSessionResult> Upload();
    IPostEndpoint<bool> Calculate();
    IFetchResultsEndpoint Fetch();
}
