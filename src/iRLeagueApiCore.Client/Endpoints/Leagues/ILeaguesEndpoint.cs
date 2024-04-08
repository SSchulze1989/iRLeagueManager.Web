using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

public interface ILeaguesEndpoint : IGetAllEndpoint<LeagueModel>
{
    Task<ClientActionResult<LeagueModel>> Post(PostLeagueModel model, CancellationToken cancellationToken = default);
    ILeagueByIdEndpoint WithId(long leagueId);
    ILeagueByNameEndpoint WithName(string name);
}
