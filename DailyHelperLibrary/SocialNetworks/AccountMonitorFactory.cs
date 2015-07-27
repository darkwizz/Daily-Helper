using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.SocialNetworks
{
    static class AccountMonitorFactory
    {
        public static SocialNetworkAccountMonitor GetMonitor(SocialNetworkAccounts accountKind, 
            string login, string password)
        {
            switch (accountKind)
            {
                case SocialNetworkAccounts.VK:
                    return new VkAccountMonitor(login, password);
                default:
                    return new SocialNetworkAccountMonitorStub();
            }
        }
    }
}
