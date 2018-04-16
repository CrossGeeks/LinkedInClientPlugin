# iOS Setup

* Install Plugin.LinkedIn package into your iOS project.

## Prerequisites
- Complete the [LinkedIn Developer Console](LinkedInDeveloperConsoleSetup.md) to include the required configuration to your LinkedIn project.

## AppDelegate.cs
- On the FinishedLaunching method just after calling global::Xamarin.Forms.Forms.Init():
```cs
     LinkedInClientManager.Initialize();
```

## Override OpenUrl method
Override the OpenUrl method from AppDelegate class:
```cs
public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
    if (CallbackHandler.ShouldHandleUrl(url))
    {
        CallbackHandler.OpenUrl(application, url, sourceApplication, annotation);
    }
    return true;
}
```


<= Back to [Table of Contents](../../README.md)
