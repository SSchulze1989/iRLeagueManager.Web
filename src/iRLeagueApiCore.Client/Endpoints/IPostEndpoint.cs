using iRLeagueApiCore.Client.Results;

namespace iRLeagueApiCore.Client.Endpoints;

public interface IPostEndpoint<TResult> : IEndpoint
{
    Task<ClientActionResult<TResult>> Post(CancellationToken cancellationToken = default);
}

public interface IPostEndpoint<TResult, TModel> : IEndpoint
{
    Task<ClientActionResult<TResult>> Post(TModel model, CancellationToken cancellationToken = default);
}
