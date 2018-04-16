# Getting Started

If developing an application that supports iOS and Android, make sure you installed the NuGet package into your PCL project and Client projects.

## Setup
* [LinkedIn Developer Console](LinkedInDeveloperConsoleSetup.md)
* [Android Setup](AndroidSetup.md)
* [iOS Setup](iOSSetup.md)

### Login

Here is an example on how to launch the login to the Google Client:

```cs
    CrossLinkedInClient.Current.LoginAsync();
```

### Logout

Here is an example on how to logout of the Google Client:

```cs
    CrossLinkedInClient.Current.Logout();
```

### Events

All async methods also trigger events:

Login event:

```cs
  CrossLinkedInClient.Current.OnLogin += (s,a)=> 
  {
     switch (a.Status)
      {
         case LinkedInActionStatus.Completed:
          //Logged in succesfully
         break;
      }
  };
```

<= Back to [Table of Contents](../README.md)