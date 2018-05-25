using Plugin.LinkedInClient.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.iOS.LinkedIn;

namespace Plugin.LinkedInClient
{
    /// <summary>
    /// Interface for $safeprojectgroupname$
    /// </summary>
	public class LinkedInClientManager : ILinkedInClientManager
    {
        public List<string> FieldsList { get; set; }

        static TaskCompletionSource<LinkedInResponse<string>> _loginTcs;

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

        void GetUserProfile()
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
                        //TODO REPLACE By Exceptions
                        var linkedInArgs =
                            new LinkedInClientResultEventArgs<string>(null, LinkedInActionStatus.Error, error.LocalizedDescription);

                        // Send the result to the receivers
					    _onLogin.Invoke(CrossLinkedInClient.Current, linkedInArgs);
                        _loginTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                    });
            }
        }

        public void GetUserProfile(List<string> fieldsList)
        {
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
					    _onLogin.Invoke(CrossLinkedInClient.Current, linkedInArgs);
                        _loginTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                    },
                    error => {
                        //TODO REPLACE By Exceptions
                        var linkedInArgs =
                            new LinkedInClientResultEventArgs<string>(null, LinkedInActionStatus.Error, error.LocalizedDescription);

                        // Send the result to the receivers
					    _onLogin.Invoke(CrossLinkedInClient.Current, linkedInArgs);
                        _loginTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                    });
            }
        }

        public bool IsLoggedIn { get; }
    }
}
