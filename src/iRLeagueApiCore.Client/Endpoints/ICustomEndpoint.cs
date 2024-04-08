using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

public interface ICustomEndpoint<T> : IPostEndpoint<T, object>, IPostEndpoint<T>, IGetEndpoint<T>, IPutEndpoint<T, object>, IDeleteEndpoint
{
    Task<ClientActionResult<IEnumerable<T>>> GetAll(CancellationToken cancellationToken = default);
}
