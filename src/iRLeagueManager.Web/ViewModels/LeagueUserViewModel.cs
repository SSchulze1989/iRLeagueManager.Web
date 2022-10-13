using iRLeagueApiCore.Common.Models.Users;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using System.Collections.Generic;

namespace iRLeagueManager.Web.ViewModels
{
    public class LeagueUserViewModel : LeagueViewModelBase<LeagueUserViewModel, LeagueUserModel>
    {
        public LeagueUserViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, LeagueUserModel model) : 
            base(loggerFactory, apiService, model)
        {
        }

        public string UserId => model.UserId;
        public string UserName { get => model.UserName; set => SetP(model.UserName, value => model.UserName = value, value); }
        public string FirstName { get => model.Firstname; set => SetP(model.Firstname, value => model.Firstname = value, value); }
        public string LastName { get => model.Lastname; set => SetP(model.Lastname, value => model.Lastname = value,value); }

        public IEnumerable<string> LeagueRoles => model.LeagueRoles;

        public async Task<StatusResult> AddRoleAsync(string role)
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            var addRoleModel = new RoleModel() { RoleName = role };
            var request = ApiService.CurrentLeague
                .Users()
                .WithId(model.UserId)
                .AddRole()
                .Post(addRoleModel);
            var result = await request;
            if (result.Success)
            {
                SetModel(result.Content);
            }

            return result.ToStatusResult();
        }

        public async Task<StatusResult> RemoveRoleAsync(string role)
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            var addRoleModel = new RoleModel() { RoleName = role };
            var request = ApiService.CurrentLeague
                .Users()
                .WithId(model.UserId)
                .RemoveRole()
                .Post(addRoleModel);
            var result = await request;
            if (result.Success)
            {
                SetModel(result.Content);
            }

            return result.ToStatusResult();
        }

        public override void SetModel(LeagueUserModel model)
        {
            base.SetModel(model);
        }
    }
}