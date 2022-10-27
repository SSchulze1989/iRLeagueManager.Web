using iRLeagueApiCore.Client.Results;
using iRLeagueApiCore.Common.Models.Users;
using iRLeagueManager.Web.Data;
using iRLeagueManager.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels
{
    public class LeagueUsersViewModel : LeagueViewModelBase<LeagueUsersViewModel>
    {
        public LeagueUsersViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            base(loggerFactory, apiService)
        {
        }

        private ObservableCollection<LeagueUserViewModel> leagueUsers = new();
        public ObservableCollection<LeagueUserViewModel> LeagueUsers { get => leagueUsers; set => Set(ref leagueUsers, value); }

        public async Task<StatusResult> LoadAsync()
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            try
            {
                Loading = true;
                var getUsersEndpoint = ApiService.CurrentLeague.Users().Get();
                var result = await getUsersEndpoint;

                if (result.Success == false)
                {
                    return result.ToStatusResult();
                }

                var users = result.Content;
                LeagueUsers = new ObservableCollection<LeagueUserViewModel>(
                    users.Select(x => new LeagueUserViewModel(LoggerFactory, ApiService, x)));
                return result.ToStatusResult();
            }
            finally
            {
                Loading = false;
            }
        }

        public async Task<StatusResult> AddUser(UserModel user)
        {
            if (ApiService.CurrentLeague == null)
            {
                return LeagueNullResult();
            }

            if (LeagueUsers.Any(x => x.UserId == user.UserId))
            {
                return StatusResult.SuccessResult();
            }

            try
            {
                Loading = true;

                var request = ApiService.CurrentLeague
                    .Users()
                    .WithId(user.UserId)
                    .AddRole()
                    .Post(new() { RoleName = "Member" });
                var result = await request;
                if (result.Success)
                {
                    return await LoadAsync();
                }
                return StatusResult.SuccessResult();
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
