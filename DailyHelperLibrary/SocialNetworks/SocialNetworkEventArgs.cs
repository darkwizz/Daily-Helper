using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.SocialNetworks
{
    public class SocialNetworkEventArgs
    {
        public SocialNetworkAccountInfo AccountInfo { get; private set; }
        public User User { get; private set; }
        public Action<string> NoitificationHandler { get; set; }

        public SocialNetworkEventArgs(SocialNetworkAccountInfo info, User user)
        {
            AccountInfo = info;
            User = user;
        }
    }
}
