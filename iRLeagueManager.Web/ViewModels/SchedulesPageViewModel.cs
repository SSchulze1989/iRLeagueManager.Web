using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels;

public class SchedulesPageViewModel : ViewModelBase
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<SchedulesPageViewModel> logger;
    private readonly LeagueApiService apiService;

    public SchedulesPageViewModel(ILoggerFactory loggerFactory, ILogger<SchedulesPageViewModel> logger, LeagueApiService apiService)
    {
        this.loggerFactory = loggerFactory;
        this.logger = logger;
        this.apiService = apiService;
        schedules = new ObservableCollection<ScheduleViewModel>();
    }

    private ObservableCollection<ScheduleViewModel> schedules;
    public ObservableCollection<ScheduleViewModel> Schedules { get => schedules; set => Set(ref schedules, value); }

}