using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using LinkedIn.Platform;
using LinkedIn.Platform.Errors;
using LinkedIn.Platform.Listeners;
using LinkedIn.Platform.Utils;
using Plugin.LinkedInClient.Shared;

namespace Plugin.LinkedInClient
{
    /// <summary>
	/// Implementation for LinkedInClient
    /// </summary>
	public class LinkedInClientManager : Java.Lang.Object, ILinkedInClientManager, IAuthListener
    {
        // Class Debug Tag
        private static string Tag = typeof(LinkedInClientManager).FullName;
        public static int AuthActivityID = Tag.GetHashCode() % Int16.MaxValue;
        public static LISessionManager LinkedInSessionManager { get; set; }
        public static Activity CurrentActivity { get; set; }

        static TaskCompletionSource<LinkedInResponse<string>> _loginTcs;
		static TaskCompletionSource<LinkedInResponse<string>> _getProfileFieldsTcs;
        
        public bool IsLoggedIn { get; }

        public static void Initialize(Activity activity)
        {
            CurrentActivity = activity;
            LinkedInSessionManager = LISessionManager.GetInstance(Application.Context);
        }

        private static EventHandler<LinkedInClientResultEventArgs<string>> _onLogin;
        public event EventHandler<LinkedInClientResultEventArgs<string>> OnLogin
        {
            add => _onLogin += value;
            remove => _onLogin -= value;
        }

        public async Task<LinkedInResponse<string>> LoginAsync()
        {
            _loginTcs = new TaskCompletionSource<LinkedInResponse<string>>();
            LinkedInSessionManager.Init(CurrentActivity, BuildScope(), true, () =>
            {
                GetUserProfile();
            }, error =>
            {
                LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                errorEventArgs.Error = LinkedInClientErrorType.SignInDefaultError;
                errorEventArgs.Message = LinkedInClientBaseException.SignInDefaultErrorMessage;
				_onError?.Invoke(CrossLinkedInClient.Current, errorEventArgs);

                // Do something with error
                _loginTcs.TrySetException(new LinkedInClientBaseException(error.ToString()));
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

        public void Logout()
        {
            LinkedInSessionManager.ClearSession();
            OnLogoutCompleted(EventArgs.Empty);
        }

        private static Scope BuildScope()
        {
            return Scope.Build(Scope.RBasicprofile, Scope.REmailaddress);
        }

        public void OnAuthError(LIAuthError result)
        {
            System.Diagnostics.Debug.WriteLine(Tag + ": Connection to the client failed with error <" + result.ToString() + "> ");
        }

        public void OnAuthSuccess()
        {
             //GetUserProfile(FieldsList);
        }

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            LISessionManager.GetInstance(Application.Context).OnActivityResult(CurrentActivity, requestCode, (int) resultCode, data);
        }

        void GetUserProfile()
        {
            var apiRequestUrl =
                "https://api.linkedin.com/v1/people/~?format=json";
            APIHelper.GetInstance(CurrentActivity).GetRequest(CurrentActivity, apiRequestUrl,
                apiResponse =>
                {
                    var linkedInArgs =
                        new LinkedInClientResultEventArgs<string>(apiResponse.ResponseDataAsString, LinkedInActionStatus.Completed, apiResponse.StatusCode.ToString());

                    // Send the result to the receivers
				    _onLogin.Invoke(CrossLinkedInClient.Current, linkedInArgs);
                    _loginTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                },
                error =>
                {
                    LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                    errorEventArgs.Error = LinkedInClientErrorType.ApiHandlerError;
                    errorEventArgs.Message = LinkedInClientBaseException.ApiHelperErrorMessage;
				    _onError?.Invoke(CrossLinkedInClient.Current, errorEventArgs);

                    _loginTcs.TrySetException(new LinkedInClientApiHelperErrorException(error.ApiErrorResponse.Message));
                });
        }

		private static EventHandler<LinkedInClientResultEventArgs<string>> _onGetUserProfile;
        public event EventHandler<LinkedInClientResultEventArgs<string>> OnGetUserProfile
        {
            add => _onGetUserProfile += value;
            remove => _onGetUserProfile -= value;
        }

        async Task<LinkedInResponse<string>> ILinkedInClientManager.GetUserProfile(List<string> fieldsList)
        {
            _getProfileFieldsTcs = new TaskCompletionSource<LinkedInResponse<string>>();

            string fields = "";

            for (int i = 0; i < fieldsList.Count; i++)
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

            APIHelper.GetInstance(CurrentActivity).GetRequest(CurrentActivity, apiRequestUrl,
                apiResponse =>
                {
                    var linkedInArgs =
                        new LinkedInClientResultEventArgs<string>(apiResponse.ResponseDataAsString, LinkedInActionStatus.Completed, apiResponse.StatusCode.ToString());

                    // Send the result to the receivers
                    _onGetUserProfile.Invoke(this, linkedInArgs);
                    _getProfileFieldsTcs.TrySetResult(new LinkedInResponse<string>(linkedInArgs));
                },
                error =>
                {
                    LinkedInClientErrorEventArgs errorEventArgs = new LinkedInClientErrorEventArgs();
                    errorEventArgs.Error = LinkedInClientErrorType.ApiHandlerError;
                    errorEventArgs.Message = LinkedInClientBaseException.ApiHelperErrorMessage;
                    _onError?.Invoke(this, errorEventArgs);

                    _getProfileFieldsTcs.TrySetException(new LinkedInClientApiHelperErrorException(error.ApiErrorResponse.Message));
                });

            return await _getProfileFieldsTcs.Task;
        }
    }
    
}
