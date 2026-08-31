using iRLeagueApiCore.Common.Models.Users;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public class UserViewModel : LeagueViewModelBase<UserViewModel, PrivateUserModel>
{
    public UserViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) :
        this(loggerFactory, apiService, new())
    {
    }

    public UserViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, PrivateUserModel model) : 
        base(loggerFactory, apiService, model)
    {
    }

    public string UserId => model.UserId;
    public string UserName => model.UserName;
    public string Firstname { get => model.Firstname; set => SetP(model.Firstname, value => model.Firstname = value, value); }
    public string Lastname { get => model.Lastname; set => SetP(model.Lastname, value => model.Lastname = value, value); }
    public string Email { get => model.Email; set => SetP(model.Email, value => model.Email = value, value); }
    public bool ShowFullname { get => !model.HideFirstnameLastname; set => SetP(!model.HideFirstnameLastname, value => model.HideFirstnameLastname = !value, value); }

    public async Task<StatusResult> LoadUser(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            Loading = true;
            var result = await ApiService.Client
                .CustomEndpoint<PrivateUserModel>($"Users/{userId}")
                .Get(cancellationToken).ConfigureAwait(false);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    public async Task<StatusResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Loading = true;
            var result = await ApiService.Client
                .Users()
                .WithId(UserId)
                .Put(MapToPutUserModel(model), cancellationToken).ConfigureAwait(false);
            if (result.Success && result.Content is not null)
            {
                SetModel(result.Content);
            }
            return result.ToStatusResult();
        }
        finally
        {
            Loading = false;
        }
    }

    private PutUserModel MapToPutUserModel(PrivateUserModel user)
    {
        return new()
        {
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            HideFirstnameLastname = user.HideFirstnameLastname
        };
    }
}
