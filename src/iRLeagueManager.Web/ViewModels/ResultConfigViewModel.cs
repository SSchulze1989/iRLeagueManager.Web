using iRLeagueApiCore.Common.Models;
using iRLeagueManager.Web.Data;

namespace iRLeagueManager.Web.ViewModels
{
    public class ResultConfigViewModel : LeagueViewModelBase<ResultConfigViewModel>
    {
        private readonly ResultConfigModel model;

        public ResultConfigViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService) : 
            this(loggerFactory, apiService, new ResultConfigModel())
        {
        }

        public ResultConfigViewModel(ILoggerFactory loggerFactory, LeagueApiService apiService, ResultConfigModel model)
            : base(loggerFactory, apiService)
        {
            this.model = model;
        }

        public string Name { get => model.Name; set => model.Name = value; }
        public string DisplayName { get => model.DisplayName; set => model.DisplayName = value; }
    }
}