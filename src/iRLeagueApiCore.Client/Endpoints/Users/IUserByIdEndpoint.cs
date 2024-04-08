using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

public interface IUserByIdEndpoint : IGetEndpoint<UserModel>, IPutEndpoint<PrivateUserModel, PutUserModel>
{
    IPostEndpoint<object> ConfirmEmail(string token);
}
