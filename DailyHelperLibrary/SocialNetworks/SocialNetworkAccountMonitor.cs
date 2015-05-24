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
        public string Login { get; private set; }
        public string Password { get; private set; }
        public Action<string> NotificationHandler { get; set; }

        public SocialNetworkAccountMonitor(string login, string password, Action<string> notificationHandler)
        {
            Login = login;
            Password = password;
            NotificationHandler = notificationHandler;
        }

        public abstract void StartMonitoring();
        public abstract void StopMonitoring();
    }
}
