using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

public interface ILeagueUsersEndpoint : IGetAllEndpoint<LeagueUserModel>
{
    ILeagueUserByIdEndpoint WithId(string id);
}
