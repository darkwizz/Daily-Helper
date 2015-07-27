using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Faults;
using DailyHelperLibrary.Savers;

namespace DailyHelperLibrary.SocialNetworks
{
    public class SocialNetworkModule
    {
        private ISocialNetworkAccountInfoSaver _proxy;

        public SocialNetworkModule(ISocialNetworkAccountInfoSaver saver)
        {
            _proxy = saver;
        }

        public EventResult OnLoggedIn(SocialNetworkEventArgs e)
        {
            try
            {
                User user = e.User;
                SocialNetworkAccountInfo info = e.AccountInfo;
                SocialNetworkAccountMonitor monitor = AccountMonitorFactory.GetMonitor(SocialNetworkAccounts.VK,
                    info.Login, info.Password);
                if (!monitor.Authorize())
                {
                    return new EventResult(false, string.Format("Can't log into account {0}", info.Login));
                }
                info.IsActive = true;
                _proxy.UpdateAccountInfo(user, info);
                ThreadPool.QueueUserWorkItem((state) =>
                    { MonitorAccount(info, monitor, e.NoitificationHandler); }, null);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }

        private void MonitorAccount(SocialNetworkAccountInfo accountInfo,
            SocialNetworkAccountMonitor monitor, Action<string> notificationHandler)
        {
            while (accountInfo.IsActive)
            {
                string response = monitor.GetServerResponse();
                if (monitor.HasNewUnreadMessages(response))
                {
                    List<string> authors = monitor.GetUnreadMessagesAuthors(response);
                    string notification = "";
                    foreach (var author in authors)
                    {
                        notification += string.Format("New message from {0}\n", author);
                    }
                    notificationHandler(notification);
                }
            }
        }

        public EventResult OnLoggedOut(SocialNetworkEventArgs e)
        {
            try
            {
                User user = e.User;
                SocialNetworkAccountInfo info = e.AccountInfo;
                SocialNetworkAccountMonitor monitor = AccountMonitorFactory.GetMonitor(SocialNetworkAccounts.VK,
                    info.Login, info.Password);
                info.IsActive = false;
                _proxy.UpdateAccountInfo(user, info);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }
    }
}
