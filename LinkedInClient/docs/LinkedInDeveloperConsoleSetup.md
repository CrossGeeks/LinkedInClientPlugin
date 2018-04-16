# LinkedIn Developer Console Setup Android & iOS
## Android Setup
**1.** First of all, we will have to create an application on the LinkedIn Developer Console. You can access it [here.](https://www.linkedin.com/secure/developer)

![Creating Application 1](https://github.com/CrossGeeks/LinkedInClientPlugin/blob/master/LinkedInClient/images/CreateApp.png?raw=true)

## Information for App
Fill in the basic information for your application in the form.

![Fill App information](https://github.com/CrossGeeks/LinkedInClientPlugin/blob/master/LinkedInClient/images/FillAppInformation.png?raw=true)

## Application Linking & Configuration
After creating your application  we will need to configurate it to link to our Android and iOS applications, for this you will need to select the **Mobile** section in the Developer Console.

![Configuring App 1](https://github.com/CrossGeeks/LinkedInClientPlugin/blob/master/LinkedInClient/images/AppSettingSelectMobile.png?raw=true)

**2.** We will need to fill out the form with the information of our App, to enable the LinkedIn Client, and to do so we will need to provide LinkedIn with the package name and a SHA-1 certificate.

![Configuring App 2](https://github.com/CrossGeeks/LinkedInClientPlugin/blob/master/LinkedInClient/images/AndroidSHAKey.png?raw=true)

For our example application we will use a debug key, but to publish your app to the store, you will need a different SHA-1 key. You can read more about it [here](https://docs.microsoft.com/en-us/xamarin/android/platform/maps-and-location/maps/obtaining-a-google-maps-api-key?tabs=vswin#Obtaining_your_Signing_Key_Fingerprint). 

For the SHA-1 debug key you will need to modify the command bellow, open a CMD or Terminal and paste the bash:

```bash
Mac: keytool -list -v -keystore /Users/[USERNAME]/.local/share/Xamarin/Mono\ for\ Android/debug.keystore -alias androiddebugkey -storepass android -keypass android

Windows: keytool -list -v -keystore "C:\Users\[USERNAME]\AppData\Local\Xamarin\Mono for Android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

```

**3.** Finally, you can press ADD > UPDATE the information of the application and now your LinkedIn App is enable for your Android Project.


## iOS Setup
We will also need to link our iOS Project, the steps are very similar to the one we just did for Android, you will to provide your **application bundle id**. This time there will be no need to add SHA-1 certificate, so go ahead and ADD > UPDATE the information of your application.

You can find your application bundle id by opening the **Info.plist** file in your iOS Project.
![iOS Application bundle id](https://github.com/CrossGeeks/GoogleClientPlugin/blob/master/GoogleClient/images/iOSInfoplistBundleID.PNG?raw=true)

Fill out the form with the required information and update the App.

![iOS Application integration](https://github.com/CrossGeeks/LinkedInClientPlugin/blob/master/LinkedInClient/images/iOSBundleID.png?raw=true)

After you have completed the configuration and linking of the LinkedIn Developer Console project you're ready for the final steps in the [Android Setup](AndroidSetup.md) and [iOS Setup](iOSSetup.md) guides.

<= Back to [Table of Contents](../../README.md)
