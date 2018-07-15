﻿using Plugin.LinkedInClient.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.iOS.LinkedIn;
using Foundation;
using UIKit;

namespace Plugin.LinkedInClient
{
    /// <summary>
    /// Interface for $safeprojectgroupname$
    /// </summary>
	public class LinkedInClientManager : ILinkedInClientManager
    {
        public List<string> FieldsList { get; set; }
		public bool IsLoggedIn { get; }
        static TaskCompletionSource<LinkedInResponse<string>> _loginTcs;
        static TaskCompletionSource<LinkedInResponse<string>> _getProfileFieldsTcs;

		public string ActiveToken 
        {
            get
            {
                if (SessionManager.HasValidSession)
                    return SessionManager.SharedInstance.Session.AccessToken.AccessTokenValue;
                return string.Empty;
            }
        }
        public DateTime TokenExpirationDate  
        {
            get
            {
                if (SessionManager.HasValidSession)
                    return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(SessionManager.SharedInstance.Session.AccessToken.Expiration.SecondsSinceReferenceDate);
                return TokenExpirationDate;
            }
        }

        private static EventHandler<LinkedInClientResultEventArgs<string>> _onLogin;
        public event EventHandler<LinkedInClientResultEventArgs<string>> OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }

        static EventHandler<LinkedInClientErrorEventArgs> _onError;
        public event EventHandler<LinkedInClientErrorEventArgs> OnError
        {
            add => _onError += value;
            remove => _onError -= value;
        }

        protected virtual void OnLinkedInClientError(LinkedInClientErrorEventArgs e)
        {
            _onError?.Invoke(CrossLinkedInClient.Current, e);
        }
        
        public async Task<LinkedInResponse<string>> LoginAsync()
        {
            _loginTcs = new TaskCompletionSource<LinkedInResponse<string>>();
            //FieldsList = fieldsList;

            SessionManager.CreateSessionWithAuth(
                new[] { Permission.BasicProfile, Permission.EmailAddress },
                "state",
                true,
                returnState =>
                {
                    GetUserProfile();
                    Debug.WriteLine("Auth Successful");
                },
                error =>
                {
                    LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                    errorEventArgs.Error = LinkedInClientErrorType.SignInDefaultError;
                    errorEventArgs.Message = LinkedInClientBaseException.SignInDefaultErrorMessage;
                    _onError?.Invoke(CrossLinkedInClient.Current, errorEventArgs);

                    // Do something with error
                    _loginTcs.TrySetException(new LinkedInClientBaseException(error.LocalizedDescription));
                });

            return await _loginTcs.Task;
        }

        static EventHandler _onLogout;
        public event EventHandler OnLogout
        {
            add => _onLogout += value;
            remove => _onLogout -= value;
        }


        protected virtual void OnLogoutCompleted(EventArgs e)
        {
            _onLogout?.Invoke(this, e);
        }

        public void Logout()
        {
            SessionManager.ClearSession();
            OnLogoutCompleted(EventArgs.Empty);
        }

        public void GetUserProfile()
        {
            if (SessionManager.HasValidSession)
            {
                var apiRequestUrl =
                    "https://api.linkedin.com/v1/people/~?format=json";

               ApiHelper.SharedInstance.GetRequest(
                    apiRequestUrl,
                    apiResponse => {
                        var linkedInArgs =
                            new LinkedInClientResultEventArgs<string>(apiResponse.Data.ToString(), LinkedInActionStatus.Completed, apiResponse.StatusCode.ToString());

                        // Send the result to the receivers
                        _onLogin.Invoke(this, linkedInArgs);
                        _loginTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                    },
                    error => {               
    					LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                        errorEventArgs.Error = LinkedInClientErrorType.SignInDefaultError;
                        errorEventArgs.Message = LinkedInClientBaseException.SignInDefaultErrorMessage;
                        _onError?.Invoke(CrossLinkedInClient.Current, errorEventArgs);

                        // Do something with error
                        _loginTcs.TrySetException(new LinkedInClientBaseException(error.LocalizedDescription));
                    });
            }
        }

		private static EventHandler<LinkedInClientResultEventArgs<string>> _onGetUserProfile;
        public event EventHandler<LinkedInClientResultEventArgs<string>> OnGetUserProfile
        {
            add => _onGetUserProfile += value;
            remove => _onGetUserProfile -= value;
        }

		public async Task<LinkedInResponse<string>> ILinkedInClientManager.GetUserProfile(List<string> fieldsList)
        {
            _getProfileFieldsTcs = new TaskCompletionSource<LinkedInResponse<string>>();

            if (SessionManager.HasValidSession)
            {
                string fields = "";

                for (int i = 0; i < fieldsList?.Count; i++)
                {
                    if (i != fieldsList.Count - 1)
                    {
                        fields += fieldsList[i] + ",";
                    }
                    else
                    {
                        fields += fieldsList[i];
                    }
                }

                var apiRequestUrl =
                    "https://api.linkedin.com/v1/people/~?format=json";

                if (fieldsList != null && fieldsList.Count > 0)
                {
                    apiRequestUrl =
                        "https://api.linkedin.com/v1/people/~:(" + fields + ")?format=json";
                }

                ApiHelper.SharedInstance.GetRequest(
                    apiRequestUrl,
                    apiResponse => {
                        var linkedInArgs =
                            new LinkedInClientResultEventArgs<string>(apiResponse.Data.ToString(), LinkedInActionStatus.Completed, apiResponse.StatusCode.ToString());

                        // Send the result to the receivers
                        _onGetUserProfile.Invoke(this, linkedInArgs);
                        _getProfileFieldsTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                    },
                    error => {
                        LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                        errorEventArgs.Error = LinkedInClientErrorType.SignInDefaultError;
                        errorEventArgs.Message = LinkedInClientBaseException.SignInDefaultErrorMessage;
                        _onError?.Invoke(this, errorEventArgs);

                        // Do something with error
                        _getProfileFieldsTcs.TrySetException(new LinkedInClientBaseException(error.LocalizedDescription));
                    });
            }
            else
            {
                LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                errorEventArgs.Error = LinkedInClientErrorType.SignInDefaultError;
                errorEventArgs.Message = LinkedInClientBaseException.SignInDefaultErrorMessage;
                _onError?.Invoke(this, errorEventArgs);

                // Do something with error
				_getProfileFieldsTcs.TrySetException(new LinkedInClientBaseException("The Session manager doesn't have a valid session."));
            }

            return await _getProfileFieldsTcs.Task;
        }

		public static bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (CallbackHandler.ShouldHandleUrl(url))
            {
                CallbackHandler.OpenUrl(application, url, sourceApplication, annotation);
            }
            return true;
        }
    }
}
