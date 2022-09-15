﻿using iRLeagueManager.Web.Data;
using MvvmBlazor.ViewModel;
using System.Collections.ObjectModel;

namespace iRLeagueManager.Web.ViewModels;

public class SchedulesPageViewModel : LeagueViewModelBase<SchedulesPageViewModel>
{
    public SchedulesPageViewModel(ILoggerFactory loggerFactory, ILogger<SchedulesPageViewModel> logger, LeagueApiService apiService) 
        : base(loggerFactory, apiService)
    {
        schedules = new ObservableCollection<ScheduleViewModel>();
    }

    private ObservableCollection<ScheduleViewModel> schedules;
    public ObservableCollection<ScheduleViewModel> Schedules { get => schedules; set => Set(ref schedules, value); }

    public async Task Load()
    {
        if (ApiService.CurrentSeason == null)
        {
            Schedules.Clear();
            return;
        }

        Loading = true;

        //await Task.Delay(500);

        var result = await ApiService.CurrentSeason.Schedules().Get();
        if (result.Success == false)
        {
            Schedules.Clear();
            // do some state reporting...
            return;
        }

        var getSchedules = result.Content;
        Schedules = new ObservableCollection<ScheduleViewModel>(getSchedules.Select(x =>
            new ScheduleViewModel(LoggerFactory, ApiService, x))
        );

        Loading = false;
    }
}