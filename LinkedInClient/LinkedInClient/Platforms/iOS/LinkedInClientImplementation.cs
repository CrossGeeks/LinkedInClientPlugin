using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.LinkedInClient
{
    /// <summary>
    /// Interface for $safeprojectgroupname$
    /// </summary>
    public class LinkedInClientImplementation : ILinkedInClientManager
    {
        public bool IsLoggedIn => throw new NotImplementedException();

        public event EventHandler<LinkedInClientResultEventArgs<string>> OnLogin;
        public event EventHandler OnLogout;

        public Task<LinkedInResponse<string>> LoginAsync(List<string> fieldsList)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
