# Events and Results



### Upload Results

!!! note
	This section shows how to the result files from iracing and upload them into iRLeagueManager.
	A direct import using the iracing data API is currently in the works and will be available in the future.

#### Export Results from iRacing

iRLeagueManager uses the `json` export feature from **iRacing** to import the results into the tool.
To get the results you need to open the **iRacing UI** and go to the result you want to export and click *Export Results*
> ![Screenshot](img/Screenshot 2023-04-03 174047.png)

Save the json file some where you can find it.

#### Upload to iRLeagueManager

1.  Go to the **Results** section of your league
	![Screenshot](img/Screenshot 2023-04-03 174621.png){width="500"}

2.  Select the Event you want to upload the result for
	![Screenshot](img/Screenshot 2023-04-03 174948.png){width="540"}

3.  Click on *Upload Result* and select the json file that you saved earlier
	![Screenshot](img/Screenshot 2023-04-03 175157.png){width="450"}

4.  You may need to refresh the page before the event results should show up

#### Re-Calculate Results

Sometime the recalculation of a result is needed, for example after you made some changes to the championship or to apply penalties from reviews.
You can simply do this by clicking on the *Calculate* button displayed above the result
> ![Screenshot](img/Screenshot 2023-04-03 175836.png){width="300"}

!!! warning
	Calculation of the results cannot be reverted and will be active immediately. Please make sure you do not accidentally lose any data before calculating.