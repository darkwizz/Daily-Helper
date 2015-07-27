using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.SocialNetworks
{
    class SocialNetworkAccountMonitorStub: SocialNetworkAccountMonitor
    {
        public SocialNetworkAccountMonitorStub(): base("StubLogin", "StubPassword")
        { }

        public override bool Authorize()
        {
            Console.WriteLine("Log in account...");
            return false;
        }

        public override string GetServerResponse()
        {
            Console.WriteLine("Returning of server response...");
            return "";
        }

        public override bool HasNewUnreadMessages(string response)
        {
            Console.WriteLine("Checking on unread messages...");
            return false;
        }

        public override List<string> GetUnreadMessagesAuthors(string response)
        {
            Console.WriteLine("Returning messages authors...");
            return new List<string>(0);
        }
    }
}
