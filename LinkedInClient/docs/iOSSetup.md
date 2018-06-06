# iOS Setup

* Install Plugin.LinkedIn package into your iOS project.

## Prerequisites
- Complete the [LinkedIn Developer Console](LinkedInDeveloperConsoleSetup.md) to include the required configuration to your LinkedIn project.

## AppDelegate.cs
### Override OpenUrl method
Override the OpenUrl method from AppDelegate class:
```cs
public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
    return LinkedInClientManager.OpenUrl(application, url, sourceApplication, annotation);
}
```


<= Back to [Table of Contents](../../README.md)
