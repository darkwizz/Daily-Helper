using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.ServiceContracts;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Proxies
{
    public class UserSaver : IUserSaver, IDisposable
    {
        private UserSaverProxy _proxy = new UserSaverProxy();

        public bool RegisterUser(User user)
        {
            return _proxy.RegisterUser(user);
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

            public bool RegisterUser(User user)
            {
                return Channel.RegisterUser(user.ServiceUser);
            }

            public User GetUser(string email)
            {
                ServiceUser user = Channel.GetUser(email);
                if (user == null)
                {
                    return null;
                }
                return user.User;
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
