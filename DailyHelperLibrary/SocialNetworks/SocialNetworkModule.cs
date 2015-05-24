using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;

namespace DailyHelperLibrary.SocialNetworks
{
    public class SocialNetworkModule
    {
        private ISocialNetworkAccountInfoSaver _proxy;

        public SocialNetworkModule(ISocialNetworkAccountInfoSaver saver)
        {
            _proxy = saver;
        }

        public EventResult OnLoggedIn(SocialNetworkEventArgs e)
        {
            User user = e.User;
            SocialNetworkAccountInfo info = e.AccountInfo;
            SocialNetworkAccountMonitor monitor = new SocialNetworkAccountMonitorStub();
            _proxy.UpdateInfo(user, info);
            monitor.StartMonitoring();
            return new EventResult(true);
        }

        public EventResult OnLoggedOut(SocialNetworkEventArgs e)
        {
            User user = e.User;
            SocialNetworkAccountInfo info = e.AccountInfo;
            SocialNetworkAccountMonitor monitor = new SocialNetworkAccountMonitorStub();
            _proxy.UpdateInfo(user, info);
            monitor.StopMonitoring();
            return new EventResult(true);
        }
    }
}
