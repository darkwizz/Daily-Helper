using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    public enum SocialNetworkAccounts { VK, Twitter, Facebook }

    public class SocialNetworkAccountInfo
    {
        private static Dictionary<SocialNetworkAccounts, ServiceSocialNetworkAccounts> _serviceAccounts = null;
        private static Dictionary<ServiceSocialNetworkAccounts, SocialNetworkAccounts> _accounts = null;

        public Guid Id { get; private set; }
        public string Login { get; internal set; }
        public string Password { get; internal set; }
        public SocialNetworkAccounts Account { get; private set; }
        public bool IsActive { get; set; }

        public SocialNetworkAccountInfo(string login, string password, SocialNetworkAccounts account)
        {
            Id = Guid.NewGuid();
            Login = login;
            Password = password;
            Account = account;
            IsActive = false;

            if (_serviceAccounts == null)
            {
                _serviceAccounts = new Dictionary<SocialNetworkAccounts, ServiceSocialNetworkAccounts>();
                _serviceAccounts.Add(SocialNetworkAccounts.Facebook, ServiceSocialNetworkAccounts.Facebook);
                _serviceAccounts.Add(SocialNetworkAccounts.Twitter, ServiceSocialNetworkAccounts.Twitter);
                _serviceAccounts.Add(SocialNetworkAccounts.VK, ServiceSocialNetworkAccounts.VK);
            }
            if (_accounts == null)
            {
                _accounts = new Dictionary<ServiceSocialNetworkAccounts, SocialNetworkAccounts>();
                _accounts.Add(ServiceSocialNetworkAccounts.Facebook, SocialNetworkAccounts.Facebook);
                _accounts.Add(ServiceSocialNetworkAccounts.Twitter, SocialNetworkAccounts.Twitter);
                _accounts.Add(ServiceSocialNetworkAccounts.VK, SocialNetworkAccounts.VK);
            }
        }

        internal SocialNetworkAccountInfo(ServiceSocialNetworkAccountInfo info)
        {
            Id = info.Id;
            Login = info.Login;
            Password = info.Password;
            IsActive = info.IsActive;
            Account = _accounts[info.ServiceAccount];
        }

        internal ServiceSocialNetworkAccountInfo ServiceAccountInfo
        {
            get
            {
                ServiceSocialNetworkAccountInfo info = new ServiceSocialNetworkAccountInfo
                {
                    Login = Login,
                    Password = Password,
                    IsActive = IsActive,
                    ServiceAccount = _serviceAccounts[Account],
                    Id = Id
                };
                return info;
            }
        }
    }
}
