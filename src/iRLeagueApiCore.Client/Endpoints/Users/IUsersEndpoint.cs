using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Users;

namespace iRLeagueApiCore.Client.Endpoints.Users;

public interface IUsersEndpoint
{
    IUserByIdEndpoint WithId(string id);
    IPostEndpoint<IEnumerable<UserModel>, SearchModel> Search();
    IPostEndpoint<object, string> ResendConfirmation();
}
