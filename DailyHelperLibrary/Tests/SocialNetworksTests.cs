using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.SocialNetworks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DailyHelperLibrary.Tests
{
    class SocialNetworksTests
    {
        [Test]
        public void TestVkMonitorFailChecking()
        {
            SocialNetworkAccountMonitor monitor = AccountMonitorFactory.GetMonitor(SocialNetworkAccounts.VK,
                "atata", "ehlo");
            // [ 4, .... ] 4 -> means that new unread message came
            string response = "{ ts: 196851352, updates: [ [ 9, -835293, 1 ], [ 9, -23498, 1 ] ] } ";
            Assert.False(monitor.HasNewUnreadMessages(response));
        }
    }
}
