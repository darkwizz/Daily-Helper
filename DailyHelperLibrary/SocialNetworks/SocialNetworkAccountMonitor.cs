using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.SocialNetworks
{
    /// <summary>
    /// Base class to social network accounts monitoring
    /// TODO: create VKAccountMonitor, TwitterAccountMonitor, FacebookAccountMonitor
    /// </summary>
    abstract class SocialNetworkAccountMonitor
    {
        protected string _email;
        protected string _password { get; private set; }
        protected Action<string> _notificationHandler { get; set; }

        public SocialNetworkAccountMonitor(string email, string password, Action<string> notificationHandler)
        {
            _email = email;
            _password = password;
            _notificationHandler = notificationHandler;
        }

        public abstract void StartMonitoring();
        public abstract void StopMonitoring();
    }
}
