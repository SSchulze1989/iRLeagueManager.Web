# Welcome to iRLeagueManager

Your home for league administration!  
A powerful web based scoring tool for iRacing leagues that offers a fast and reliable league database, support for many race types and scoring configurations and a 
built in stewarding system.

!!! note
    Registration has not opened yet but you can already try it out if you join my discord and shoot me a message:  
    [https://discord.gg/XUG2kad](https://discord.gg/XUG2kad)

### Description
Online sim racing has gained strong popularity among hobbyists and professionals alike and iRacing is one of the leading platforms for competitive racing.
Many people choose to organize private hosted races in leagues to stir up the competition, recreate historical championships or just to have fun with friends.

While iRacing does provide support for leagues and league scoring the capabilities fall short compared to the variety of race formats and points calculation formats 
that people want to implement for their league.
As a result, many chose to export the results from iRacing and use a 3rd party solution to manage and host their scorings. Solutions range from using spreadsheets 
up to full scale result hosters.
Managing spreadsheets can be a tedious and time consuming task and as the leagues grow the workload for keeping up with current and past results will grow larger. 
Oftentimes leagues implement some kind of penalty system that also needs to be accounted for through points deduction or time penalties.

**iRLeagueManager is providing such a service for scoring and hosting your league results all-in-one.**

> <img src="https://user-images.githubusercontent.com/57634354/210998860-b9859c2f-2d07-4d4d-8981-0b47242aba51.png" width="75%" />  

Once configured the software can generate results and standings, apply penalty points and publish to your website all with the click of one button, 
you can generate extensive statistics and metrics spanning over years of races with minimal amount of effort and the build in reviews and stewarding 
system gives a home to after race discussions about incidents and can also apply automatic penalty points based on a incident category system.
Finally you can either use the webapp directly to share the results with the world or if you already have a league webpage use the API to publish the data in real time.

### Features
A list of supported features by now. The app is in constant development and more features will be added over time. If you miss something for your league don't hesitate to open up a feature request.

- Automated scoring and standings calculation
- Support for heat races, team races and multiclass formats.
    - Flexible scoring configurations for a variety of use cases including
        - Different points calculations methods
        - Multiple result configurations per event (eg. Pro/AM or Multiclass scoring)
        - Filters to generate custom result set
    - Customizable sorting
    - Bonus points (position, pole, least incidents ...)
    - Automated penalty points for incidents
- Fully open REST API: access 100% of your league data and publish them to your own webpage
  > Access documentation at: [https://irleaguemanager.net/irleagueapi/swagger/index.html](https://irleaguemanager.net/irleagueapi/swagger/index.html)
  > Checkout [https://github.com/SSchulze1989/iRLeagueApiCore](https://github.com/SSchulze1989/iRLeagueApiCore)
- Role based user access: Manage admins, organizers and stewards through the role based access system
- Review system  
    - Register incidents and discuss with other stewards before finalizing the results  
      > <img src="https://user-images.githubusercontent.com/57634354/211000455-c8b03fc2-0635-4981-81ca-a22907711891.png" width="75%" />
    - Automatic penalty points for incident categories
    - Protest form to let drivers report incidents from the previous race  