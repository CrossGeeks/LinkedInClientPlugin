# Android Setup

* Install Plugin.LinkedIn package into your Android project.

## Prerequisites
- Compatible Android device that runs Android 4.0+ and includes the LinkedIn Official App or an emulator with an AVD that runs the LinkedIn Official App based on Android 4.2+
- The latest version of the Android SDK, including the SDK Tools component. You can get the latest SDK from the Android SDK Manager in Visual Studio.
- Project configured to compile against Android 4.0(Ice Cream Sandwich) or newer.

- Complete the [LinkedIn Developer Console](LinkedInDeveloperConsoleSetup.md) to include the required configuration to your LinkedIn project.

## MainActivity.cs

- On the **OnCreate** method just after calling base.OnCreate:
```cs
     LinkedInClientManager.Initialize(this);
```

- Override the **OnActivityResult** method:
```cs
protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);
    LinkedInClientManager.OnActivityResult(requestCode, resultCode, data);
}
```

## AndroidManifest.xml

Add this permission.
```xml
    <uses-permission android:name="android.permission.INTERNET"/>
```


<= Back to [Table of Contents](../../README.md)
