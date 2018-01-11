[![N|Solid](https://testingasaservice.io/img/logo-testingdigital-by-ByronGroup.png)](https://testingdigital.com/)
# Automation Baseline Progressive Web App 
. A basic example for getting started with automated tests on real devices for Progressive web application (WPA).

  - Test case from : [Progressive Web App Checklist]
  - Real Devices!.
  - Easely scallable, with Ressource to use as you want.

# Workspace

  - [VisualStudio 2017]
  - [NodeJS]
  - [Appium 1.7] use [Appium Doctor] to setup.
  - [Android SDK] (Ensure to set adb as envirennement variable)
  - Android device with developer mode and connected to the machine!.
  
# Installation
  - Download the repository. 
  - Start appium server `appium -a "YOUR_IP" -p 477`
   ![N|Solid](https://imgur.com/KOa4Ygi.png)
  - Start the solution with Visual Studio 2017.
  - Build and run test(s) using `Tests explorer`. 
  
   ![N|Solid](https://imgur.com/rFaA8ik.png)

# Fixture.cs
Set your variables! 
```sh
udid = "YOUR DEVICE UDID";
```
```sh
"Add list of your website & subUrls to test";
List<string> UrlsToTest... 
```
# Ressource.cs
>Definition of the object that we are using to get all data from a WebPage, WebElements, Tags, Scripts, LoadTime, HttpResponse object .....

# Script.cs
>Test cases, your can override them, add methods as much as you want for your own test.
>Using the Ressource object you can get almost all relevent data to use for you ergonomique and performance and network test.

   [Progressive Web App Checklist]: <https://developers.google.com/web/progressive-web-apps/checklist>
   [VisualStudio 2017]: <https://www.visualstudio.com/fr/downloads/?rr=https%3A%2F%2Fwww.google.fr%2F>
   [Appium 1.7]: <https://libraries.io/npm/appium/1.7.1>
   [Appium doctor]: <https://libraries.io/npm/appium-doctor/>
   [NodeJs]: <https://nodejs.org/en/>
   [Android SDK]: <https://developer.android.com/studio/index.html>



