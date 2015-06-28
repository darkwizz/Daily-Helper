using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.SocialNetworks
{
    class SocialNetworkAccountMonitorStub: SocialNetworkAccountMonitor
    {
        public SocialNetworkAccountMonitorStub(): base("StubLogin", "StubPassword", StubHandler)
        { }

        private static void StubHandler(string temp)
        {
            Console.WriteLine("New notification: " + temp);
        }

        public override void StartMonitoring()
        {
            Console.WriteLine("Start monitoring of default stub account...");
        }

        public override void StopMonitoring()
        {
            Console.WriteLine("Stop monitoring of default stub account...");
        }
    }
}
