using iRLeagueApiCore.Common;
using iRLeagueApiCore.Common.Models.Users;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.ViewModels;

public sealed class LeagueUserViewModel : LeagueViewModelBase<LeagueUserViewModel, LeagueUserModel>
{
    public LeagueUserViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, LeagueUserModel model) :
        base(loggerFactory, apiService, model)
    {
    }

    public string UserId => model.UserId;
    public string UserName { get => model.UserName; set => SetP(model.UserName, value => model.UserName = value, value); }
    public string FirstName { get => model.Firstname; set => SetP(model.Firstname, value => model.Firstname = value, value); }
    public string LastName { get => model.Lastname; set => SetP(model.Lastname, value => model.Lastname = value, value); }

    public IEnumerable<LeagueRoleValue> Roles => model.LeagueRoles.Select(x => LeagueRoles.GetRoleValue(x));

    public bool HasRole(LeagueRoleValue role)
    {
        return LeagueRoles.CheckRole(role, Roles);
    }

    public bool IsImplicitRole(LeagueRoleValue role)
    {
        var implicitOfRoles = LeagueRoles.ImplicitRoleOf(role);
        return implicitOfRoles.Any(x => LeagueRoles.CheckRole(x, Roles));
    }

    public async Task<StatusResult> AddRoleAsync(LeagueRoleValue role)
    {
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        var addRoleModel = new RoleModel() { RoleName = role.ToString() };
        var request = ApiService.CurrentLeague
            .Users()
            .WithId(model.UserId)
            .AddRole()
            .Post(addRoleModel);
        var result = await request;
        if (result.Success && result.Content is not null)
        {
            SetModel(result.Content);
        }

        return result.ToStatusResult();
    }

    public async Task<StatusResult> RemoveRoleAsync(LeagueRoleValue role)
    {
        if (ApiService.CurrentLeague == null)
        {
            return LeagueNullResult();
        }

        var addRoleModel = new RoleModel() { RoleName = role.ToString() };
        var request = ApiService.CurrentLeague
            .Users()
            .WithId(model.UserId)
            .RemoveRole()
            .Post(addRoleModel);
        var result = await request;
        if (result.Success && result.Content is not null)
        {
            SetModel(result.Content);
        }

        return result.ToStatusResult();
    }

    protected override void SetModel(LeagueUserModel model)
    {
        base.SetModel(model);
    }
}
