using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.LinkedInClient
{
	public enum LinkedInClientErrorType
    {
        SignInDefaultError,
        ApiHandlerError
    }

    public class LinkedInClientErrorEventArgs : EventArgs
    {
        public LinkedInClientErrorType Error { get; set; }
        public string Message { get; set; }
    }

    public enum LinkedInClientScopes
    {
        EmailAddress,
        BasicProfile,
        ContactInfo,
        FullProfile,
        CompanyAdmin,
        Share
    }

    public enum LinkedInActionStatus
    {
        Canceled,
        Unauthorized,
        Completed,
        Error
    }

    public class LinkedInClientResultEventArgs<T> : EventArgs
    {
        public T Data { get; set; }
        public LinkedInActionStatus Status { get; set; }
        public string Message { get; set; }

        public LinkedInClientResultEventArgs(T data, LinkedInActionStatus status, string msg = "")
        {
            Data = data;
            Status = status;
            Message = msg;
        }
    }

    public class LinkedInResponse<T>
    {
        public T Data { get; set; }
        public LinkedInActionStatus Status { get; set; }
        public string Message { get; set; }

        public LinkedInResponse(LinkedInClientResultEventArgs<T> evtArgs)
        {
            Data = evtArgs.Data;
            Status = evtArgs.Status;
            Message = evtArgs.Message;
        }

        public LinkedInResponse(T user, LinkedInActionStatus status, string msg = "")
        {
            Data = user;
            Status = status;
            Message = msg;
        }
    }


    /// <summary>
    /// Interface for LinkedInClientManager
    /// </summary>
    public interface ILinkedInClientManager
    {
        event EventHandler<LinkedInClientResultEventArgs<string>> OnLogin;
        event EventHandler OnLogout;
        Task<LinkedInResponse<string>> LoginAsync();
        event EventHandler<LinkedInClientErrorEventArgs> OnError;
        void GetUserProfile(List<string> fieldsList);
        void Logout();
        bool IsLoggedIn { get; }
    }
}
