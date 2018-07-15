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

This method returns a LinkedInResponse<string> which contains the basic profile of the user that was authenticated as a raw Json on it's Data Property, this response class has the following structure:
```cs
    public class LinkedInResponse<T>
    {
        public T Data { get; set; }
        public LinkedInActionStatus Status { get; set; }
        public string Message { get; set; }
    }
```

### Logout

Here is an example on how to logout of the Google Client:

```cs
    CrossLinkedInClient.Current.Logout();
```

### Get Extra Fields from User Profile
You have a public method available that allows you to get extra information you might need from the user profile, just send a list of strings with the fields you require. ([List of available fields](https://developer.linkedin.com/docs/fields/basic-profile))
```cs
    List<string> fieldsList = new List<string> { "first-name", "last-name", "email-address", "picture-url" };
    try
    {
        await LinkedInClientManager.GetUserProfile(fields);
    }
    catch (LinkedInClientBaseException exception)
    {
        
    }
```

### Available Properties
- **ActiveToken** (Signed In user Token)
- **TokenExpirationDate** (Signed In user Token Expiration Date)

### Events

All async methods also trigger events:

Login event:

```cs
    // Available Events
    event EventHandler<LinkedInClientResultEventArgs<string>> OnLogin;
    event EventHandler<LinkedInClientResultEventArgs<string>> OnGetUserProfile;
    event EventHandler<LinkedInClientErrorEventArgs> OnError;

    //Example
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

### Exceptions
```cs
    // Indicates the Sign in process couldn't complete it's process correctly
    LinkedInClientBaseException
    
    // Indicates an error with the API has occured.
    LinkedInClientApiHelperErrorException
```

<= Back to [Table of Contents](../README.md)