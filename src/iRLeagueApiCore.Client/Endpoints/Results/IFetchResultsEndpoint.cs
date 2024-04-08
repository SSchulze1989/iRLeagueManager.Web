namespace iRLeagueApiCore.Client.Endpoints.Results;
public interface IFetchResultsEndpoint
{
    public IPostEndpoint<bool> FromIracingSubSession(int subSessionId);
}
