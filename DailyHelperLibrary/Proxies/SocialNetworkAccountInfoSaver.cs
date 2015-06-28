using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Proxies
{
    public class SocialNetworkAccountInfoSaver : ISocialNetworkAccountInfoSaver, IDisposable
    {
        private SocialNetworkAccountInfoSaverProxy _proxy = new SocialNetworkAccountInfoSaverProxy();

        public void UpdateAccountInfo(User user, SocialNetworkAccountInfo info)
        {
            _proxy.UpdateAccountInfo(user, info);
        }

        public void Dispose()
        {
            _proxy.Close();
        }

        class SocialNetworkAccountInfoSaverProxy : ClientBase<ISocialNetworkAccountInfoSaverService>
        {
            public SocialNetworkAccountInfoSaverProxy() :
                base("SaveAccountInfoEndpoint")
            { }

            public void UpdateAccountInfo(User user, SocialNetworkAccountInfo info)
            {
                Channel.UpdateAccountInfo(user.ServiceUser, info.ServiceAccountInfo);
            }

            new public void Close()
            {
                try
                {
                    base.Close();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Can't close connection. " + ex.Message); // logging
                }
            }
        }
    }
}
