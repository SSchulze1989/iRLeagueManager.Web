using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Teams;

public interface ITeamByIdEndpoint : IUpdateEndpoint<TeamModel, PutTeamModel>
{
}
