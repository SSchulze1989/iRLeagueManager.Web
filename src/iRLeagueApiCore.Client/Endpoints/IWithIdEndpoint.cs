using iRLeagueApiCore.Client.Endpoints.Teams;

namespace iRLeagueApiCore.Client.Endpoints;

public interface IWithIdEndpoint<TEndpoint, TId>
{
    TEndpoint WithId(TId id);
}

public interface IWithIdEndpoint<T> : IWithIdEndpoint<T, long>
{
}
