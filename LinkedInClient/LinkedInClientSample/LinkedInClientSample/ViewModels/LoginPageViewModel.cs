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
		public LinkedInUser user { get; set; } = new LinkedInUser();
		LinkedInClientManager LinkedInClientManager { get; set; } = (Plugin.LinkedInClient.LinkedInClientManager)CrossLinkedInClient.Current;

		public event PropertyChangedEventHandler PropertyChanged;

		public string Name
        {
            get { return user.Name; }
            set { user.Name = value; }
        }

        public string Email
        {
            get { return user.Email; }
            set { user.Email = value; }
        }

        public Uri Picture
        {
            get { return user.Picture; }
            set { user.Picture = value; }
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

            LinkedInClientManager.OnError += (sender, e) =>
            {

            };

            LinkedInClientManager.OnGetUserProfile += (sender, e) =>
            {
                var data = JObject.Parse(e.Data);
                user.Email = data["emailAddress"].ToString();
                user.Picture = new Uri(data["pictureUrl"].ToString());
                var token = LinkedInClientManager.ActiveToken;
                var expirationDate = LinkedInClientManager.TokenExpirationDate;
            };


            List<string> fieldsList = new List<string> { "first-name", "last-name", "email-address", "picture-url" };
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

        private void OnAuthError(object sender, LinkedInClientErrorEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
        }

        private void OnLoginCompleted(object sender, LinkedInClientResultEventArgs<string> linkedInClientResultEventArgs)
        {
            if (linkedInClientResultEventArgs.Data != null)
            {
                Debug.WriteLine("JSON RESPONSE: " + linkedInClientResultEventArgs.Data);
                var data = JObject.Parse(linkedInClientResultEventArgs.Data);

                user.Name = data["firstName"] + " " + data["lastName"];

                List<string> fieldsList = new List<string> { "first-name", "last-name", "email-address", "picture-url" };
                GetCustomProfileCommand.Execute(fieldsList);
                //user.Email = data["emailAddress"].ToString();
                //user.Picture = new Uri(data["pictureUrl"].ToString());
                // App.Current.MainPage.DisplayAlert("Success", "It works!", "OK");

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

        private void OnGetProfileCompleted(object sender, LinkedInClientResultEventArgs<string> linkedInClientResultEventArgs)
        {
            if (linkedInClientResultEventArgs.Data != null)
            {
                Debug.WriteLine("JSON RESPONSE: " + linkedInClientResultEventArgs.Data);
                var data = JObject.Parse(linkedInClientResultEventArgs.Data);

                user.Name = data["firstName"] + " " + data["lastName"];
                user.Email = data["emailAddress"].ToString();
                user.Picture = new Uri(data["pictureUrl"].ToString());
                // App.Current.MainPage.DisplayAlert("Success", "It works!", "OK");
                //IsLoggedIn = true;
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", linkedInClientResultEventArgs.Message, "OK");
            }
            LinkedInClientManager.OnGetUserProfile -= OnGetProfileCompleted;
            LinkedInClientManager.OnError -= OnAuthError;
        }

        private void OnGetProfileError(object sender, LinkedInClientErrorEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
        }

        public void Logout()
        {
            LinkedInClientManager.OnLogout += OnLogoutCompleted;
            LinkedInClientManager.Logout();
        }

        private void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            IsLoggedIn = false;
            LinkedInClientManager.OnLogout -= OnLogoutCompleted;
        }
	}
}
