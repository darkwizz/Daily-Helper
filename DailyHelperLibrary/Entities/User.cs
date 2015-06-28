using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    public class User
    {
        private Dictionary<SocialNetworkAccounts, SocialNetworkAccountInfo> _accounts;
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; set; }
        public Dictionary<Guid, Note> Notes { get; private set; }
        public Dictionary<Guid, TodoItem> TodoItems { get; private set; }
        public Dictionary<Guid, OnceRunningScheduleItem> ScheduleItems { get; private set; }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
            Id = Guid.NewGuid();
            Notes = new Dictionary<Guid, Note>();
            TodoItems = new Dictionary<Guid, TodoItem>();
            ScheduleItems = new Dictionary<Guid, OnceRunningScheduleItem>();

            // Init all available accounts by default values
            _accounts = new Dictionary<SocialNetworkAccounts, SocialNetworkAccountInfo>();
            foreach (var account in Enum.GetValues(typeof(SocialNetworkAccounts)))
            {
                var key = (SocialNetworkAccounts)account;
                _accounts.Add(key, new SocialNetworkAccountInfo("", "", key));
            }
        }

        public void SetAccountState(SocialNetworkAccounts account, bool isActive)
        {
            if (!HasAccount(account))
            {
                Console.WriteLine("User hasn't this account"); // logging
                return;
            }
            _accounts[account].IsActive = isActive;
        }

        public void UpdateAccountInfo(SocialNetworkAccounts account, string login, string password)
        {
            _accounts[account].Login = login;
            _accounts[account].Password = password;
        }

        public bool HasAccount(SocialNetworkAccounts account)
        {
            return _accounts[account].Login != "";
        }

        public IEnumerable<SocialNetworkAccountInfo> Accounts
        {
            get
            {
                IEnumerable<SocialNetworkAccountInfo> accounts = from item in _accounts.Values
                                                                 where item.Login != ""
                                                                 select item;
                return accounts;
            }
        }

        internal User(ServiceUser user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
            ScheduleItems = new Dictionary<Guid, OnceRunningScheduleItem>();

            Notes = new Dictionary<Guid, Note>();
            foreach (var note in user.Notes.Values)
            {
                Note newNote = note.Note;
                Notes.Add(newNote.Id, newNote);
            }

            TodoItems = new Dictionary<Guid, TodoItem>();
            foreach (var item in user.TodoItems.Values)
            {
                TodoItem newItem = item.TodoItem;
                TodoItems.Add(newItem.Id, newItem);
            }

            _accounts = new Dictionary<SocialNetworkAccounts, SocialNetworkAccountInfo>();
            foreach (var account in user.Accounts.Values)
            {
                SocialNetworkAccountInfo info = account.AccountInfo;
                _accounts.Add(info.Account, info);
            }
        }

        internal ServiceUser ServiceUser
        {
            get
            {
                ServiceUser user = new ServiceUser
                {
                    Id = Id,
                    Email = Email,
                    Password = Password
                };

                Dictionary<Guid, ServiceNote> serviceNotes = new Dictionary<Guid, ServiceNote>();
                foreach (var note in Notes.Values)
                {
                    serviceNotes.Add(note.Id, note.ServiceNote);
                }
                Dictionary<Guid, ServiceTodoItem> serviceTodoItems = new Dictionary<Guid, ServiceTodoItem>();
                foreach (var item in TodoItems.Values)
                {
                    serviceTodoItems.Add(item.Id, item.ServiceTodoItem);
                }
                Dictionary<ServiceSocialNetworkAccounts, ServiceSocialNetworkAccountInfo> serviceAccountsInfo =
                    new Dictionary<ServiceSocialNetworkAccounts, ServiceSocialNetworkAccountInfo>();
                foreach (var info in _accounts.Values)
                {
                    ServiceSocialNetworkAccountInfo serviceInfo = info.ServiceAccountInfo;
                    serviceAccountsInfo.Add(serviceInfo.ServiceAccount, serviceInfo);
                }

                user.Notes = serviceNotes;
                user.TodoItems = serviceTodoItems;
                user.Accounts = serviceAccountsInfo;
                return user;
            }
        }
    }
}
