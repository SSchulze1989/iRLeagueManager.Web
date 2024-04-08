using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Register;
public interface IAuthenticateEndpoint
{
    IPostEndpoint<UserModel, RegisterModel> Register();
}
