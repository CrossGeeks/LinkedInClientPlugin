using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using LinkedInClientSample.Models;
using Newtonsoft.Json.Linq;
using Plugin.LinkedInClient;
using Plugin.LinkedInClient.Shared;
using Xamarin.Forms;

namespace LinkedInClientSample.ViewModels
{
	public class LoginPageViewModel : INotifyPropertyChanged
    {
		public LinkedInUser User { get; set; } = new LinkedInUser();
		LinkedInClientManager LinkedInClientManager { get; set; } = (LinkedInClientManager)CrossLinkedInClient.Current;

		public event PropertyChangedEventHandler PropertyChanged;

		public string Name
        {
            get { return User.Name; }
            set { User.Name = value; }
        }

        public string Email
        {
            get { return User.Email; }
            set { User.Email = value; }
        }

        public Uri Picture
        {
            get { return User.Picture; }
            set { User.Picture = value; }
        }

        public bool IsLoggedIn { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand GetCustomProfileCommand { get; set; }

        public LoginPageViewModel()
        {
			LoginCommand = new Command(LoginAsync);
            LogoutCommand = new Command(Logout);
			GetCustomProfileCommand = new Command<List<string>>(GetCustomUserProfile);
        }

		public async void LoginAsync()
        {
			LinkedInClientManager.OnLogin += OnLoginCompleted;
            LinkedInClientManager.OnError += OnAuthError;
            
            // Example of OnError Event
			LinkedInClientManager.OnError += async (sender, exception) =>
            {
				await App.Current.MainPage.DisplayAlert("Error", exception.Message, "OK");
                LinkedInClientManager.OnLogin -= OnLoginCompleted;
                LinkedInClientManager.OnError -= OnAuthError;
            };

            // Example of GetUserProfile successfull Event
            LinkedInClientManager.OnGetUserProfile += (sender, e) =>
            {
                var data = JObject.Parse(e.Data);
                User.Email = data["emailAddress"].ToString();
                User.Picture = new Uri(data["pictureUrl"].ToString());
                var token = LinkedInClientManager.ActiveToken;
                var expirationDate = LinkedInClientManager.TokenExpirationDate;
            };

            try
            {
                await LinkedInClientManager.LoginAsync();
            }
            catch (LinkedInClientBaseException exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", exception.Message, "OK");
                LinkedInClientManager.OnLogin -= OnLoginCompleted;
                LinkedInClientManager.OnError -= OnAuthError;
            }
        }

        void OnAuthError(object sender, LinkedInClientErrorEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
        }

        // Method executed if the login was successful
        void OnLoginCompleted(object sender, LinkedInClientResultEventArgs<string> linkedInClientResultEventArgs)
        {
            if (linkedInClientResultEventArgs.Data != null)
            {
                Debug.WriteLine("JSON RESPONSE: " + linkedInClientResultEventArgs.Data);
                var data = JObject.Parse(linkedInClientResultEventArgs.Data);

                User.Name = data["firstName"] + " " + data["lastName"];
	
                // Example use of custom profile method
                List<string> fieldsList = new List<string> { "first-name", "last-name", "email-address", "picture-url" };
                GetCustomProfileCommand.Execute(fieldsList);

                IsLoggedIn = true;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", linkedInClientResultEventArgs.Message, "OK");
            }
            LinkedInClientManager.OnLogin -= OnLoginCompleted;
            LinkedInClientManager.OnError -= OnAuthError;
        }


        public async void GetCustomUserProfile(List<string> fields)
        {
            LinkedInClientManager.OnGetUserProfile += OnGetProfileCompleted;
            LinkedInClientManager.OnError += OnGetProfileError;
            try
            {
                await LinkedInClientManager.GetUserProfile(fields);
            }
            catch (LinkedInClientBaseException exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", exception.Message, "OK");
                LinkedInClientManager.OnGetUserProfile -= OnGetProfileCompleted;
                LinkedInClientManager.OnError -= OnGetProfileError;
            }
        }

		// Method executed of the GetProfile Event
        void OnGetProfileCompleted(object sender, LinkedInClientResultEventArgs<string> linkedInClientResultEventArgs)
        {
            if (linkedInClientResultEventArgs.Data != null)
            {
                Debug.WriteLine("JSON RESPONSE: " + linkedInClientResultEventArgs.Data);
                var data = JObject.Parse(linkedInClientResultEventArgs.Data);

                User.Name = data["firstName"] + " " + data["lastName"];
                User.Email = data["emailAddress"].ToString();
                User.Picture = new Uri(data["pictureUrl"].ToString());
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", linkedInClientResultEventArgs.Message, "OK");
            }
            LinkedInClientManager.OnGetUserProfile -= OnGetProfileCompleted;
            LinkedInClientManager.OnError -= OnAuthError;
        }

        // Method to receive errors of the GetProfile call
        void OnGetProfileError(object sender, LinkedInClientErrorEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
        }

        public void Logout()
        {
            LinkedInClientManager.OnLogout += OnLogoutCompleted;
            LinkedInClientManager.Logout();
        }

        // Example Logout Event
        void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            IsLoggedIn = false;
            LinkedInClientManager.OnLogout -= OnLogoutCompleted;
        }
	}
}
