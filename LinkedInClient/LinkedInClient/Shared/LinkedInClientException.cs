using System;
namespace Plugin.LinkedInClient.Shared
{
	public class  LinkedInClientBaseException : Exception
    {
        public const string SignInDefaultErrorMessage = "The LinkedIn Sign In could not complete it's process correctly.";
        public const string ApiHelperErrorMessage = "The LinkedIn Sign In could not complete it's process correctly.";

        public LinkedInClientBaseException() : base() { }
        public LinkedInClientBaseException(string message) : base(message) { }
        public LinkedInClientBaseException(string message, System.Exception inner) : base(message, inner) { }
    }

    // Indicates an error with the API has occured.
    public class LinkedInClientApiHelperErrorException : LinkedInClientBaseException
    {
        public LinkedInClientApiHelperErrorException() : base(ApiHelperErrorMessage) { }
        public LinkedInClientApiHelperErrorException(string message) : base(message) { }
        public LinkedInClientApiHelperErrorException(string message, System.Exception inner) : base(message, inner) { }
    }
}
