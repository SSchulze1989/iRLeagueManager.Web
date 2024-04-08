using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Client.Endpoints.Teams;

public interface ITeamsEndpoint : IPostGetAllEndpoint<TeamModel, PostTeamModel>, IWithIdEndpoint<ITeamByIdEndpoint>
{ 
}
