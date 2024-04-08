using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Leagues;

public interface ILeagueByIdEndpoint : IUpdateEndpoint<LeagueModel, PutLeagueModel>
{
    IPostEndpoint<LeagueModel> Initialize();
}
