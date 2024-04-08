namespace iRLeagueApiCore.Client.Endpoints;

public interface IPostGetAllEndpoint<TResult, TPost> : IPostEndpoint<TResult, TPost>, IGetAllEndpoint<TResult>
{
}
