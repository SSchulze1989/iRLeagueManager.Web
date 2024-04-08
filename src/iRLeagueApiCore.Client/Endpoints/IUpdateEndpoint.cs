namespace iRLeagueApiCore.Client.Endpoints;

public interface IUpdateEndpoint<TResult, TModel> : IGetEndpoint<TResult>, IPutEndpoint<TResult, TModel>, IDeleteEndpoint
{
}
