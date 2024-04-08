using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

public interface ILeagueUserByIdEndpoint : IGetEndpoint<LeagueUserModel>
{
    IPostEndpoint<LeagueUserModel, RoleModel> AddRole();
    IPostEndpoint<LeagueUserModel, RoleModel> RemoveRole();
}
