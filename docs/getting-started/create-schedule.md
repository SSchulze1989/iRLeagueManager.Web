# Create schedule

Before you can upload your results you need to create a schedule for your season. This is done on the **Schedule** page of your league.
You can access the schedule page by clicking on the **Schedule** link in the main navigation menu.

![Schedule Menu](img/menu_schedule_light.png#only-light){width="200px"}
![Schedule Menu](img/menu_schedule_dark.png#only-dark){width="200px"}

## Create Events

### Name, Date & Time

At first you need to fill in the basic event information like date and time of event.
Optionally you may also specify an Event name for easier identifiaction.

The date and time will be set in your local time zone so you should not have to worry about conversions.
The start time should ideally coincide with the start of the session in iRacing.

### Track & Config

Next step is to select the track and config. You can search for tracks by name and/or select them from the dropdown.
For each track the available configurations will be shown and can be selected after the track selection.

![Event Track Config](img/event_track_config_light.png#only-light){width="450"}
![Event Track Config](img/event_track_config_dark.png#only-dark){width="450"}

### Sessions

And event is divided into multiple sessions that run in succession.

    Practice -> Qualifying -> Race (-> Race#2)

Each session can be individually configured by duration and maximum number of laps.

#### Practice and Qualifying

You can optionally define attached practice and qualifying sessions for your event.
Just check the box for either practice/qualy and then enter the desired duration time or the number of laps (or both)

![Event Practice Qualifying](img/event_practice_qualy_light.png#only-light){width="450"}
![Event Practice Qualifying](img/event_practice_qualy_dark.png#only-dark){width="450"}

!!! note
    While there is no problem with uploading results from iRacing sessions that have a different 
    configuration for either practice or qualifiying it is still recommended to use the same
    configuration as in iRacing so the start and duration times match.

#### Race Session(s)

Finally configure the race mode of your event. Typically there is two main constellations that you want to run your race in:

1. **Single Race**
2. **Multi Race (Heats)**

Both can be configured the same in *iRLeagueManager* by simply adding the desired number of race sessions for the event.

![Event Race Sessions](img/event_race_sessions_light.png#only-light){width=450px}
![Event Race Sessions](img/event_race_sessions_dark.png#only-dark){width=450px}

#### Event Result Configurations

In order to produce the correct results for your event you need to select the according result configurations.
Read more about managing result configurations visit  
-> [Result Configurations](settings/championships#result-configurations)

#### Creating multiple Events
!!! note
    If you are creating multiple events in succession the settings from the previous event will be copied to the new event automatically.
    That way you will not have to type everything again and can reuse the settings.

To create events in the schedule simply click on the **Add Event** button at the top of the schedule page.

![Add Event Button](img/schedule_add_event_light.png#only-light){width="400px"}
![Add Event Button](img/schedule_add_event_dark.png#only-dark){width="400px"}

A popup will open where you can enter the details of your event. You can enter the following information:

- **Name**: The name of the event. This will be displayed in the schedule and results.
- **Date & Time**: The date and time of the event. This will be used to sort the events in the schedule.
- **Track & Config**: The track and configuration for the event. You can search for tracks by name and select the desired configuration.
- **Sessions**: The sessions that make up the event. You can pick **practice** and **qualifying** sessions and add one or **race** sessions. Each session can be configured with a duration and a maximum number of laps.
- **Event Result Configurations**: The result configurations that will be used to calculate the results for the event. You can select from the available result configurations in your league.

![Schedule Event Settings](img/schedule_event_settings_light.png#only-light){width="500px"}
![Schedule Event Settings](img/schedule_event_settings_dark.png#only-dark){width="500px"}

Fill out the event details and click on the **Save** button to save the event.

## Check Schedule

After adding all desired event your schedule schould look similar to this:

![Schedule Overview](img/schedule_overview_light.png#only-light){width="600px"}
![Schedule Overview](img/schedule_overview_dark.png#only-dark){width="600px"}