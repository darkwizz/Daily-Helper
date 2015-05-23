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
    public class UserSaver : IUserSaver, IDisposable
    {
        private UserSaverProxy _proxy = new UserSaverProxy();

        public void RegisterUser(User user)
        {
            _proxy.RegisterUser(user);
        }

        public User GetUser(string email)
        {
            return _proxy.GetUser(email);
        }

        public void Dispose()
        {
            _proxy.Close();
        }

        class UserSaverProxy : ClientBase<IUserSaverService>
        {
            public UserSaverProxy() :
                base("SaveUserEndpoint")
            { }

            public void RegisterUser(User user)
            {
                Channel.RegisterUser(user.ServiceUser);
            }

            public User GetUser(string email)
            {
                return Channel.GetUser(email).User;
            }

            new public void Close()
            {
                try
                {
                    base.Close();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Some problems with connection. " + ex.Message); // logging
                }
            }
        }
    }
}
