using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Proxies;
using DailyHelperLibrary.Entities;
using System.Net.Mail;
using System.ServiceModel;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.Scheduler;

namespace DailyHelperLibrary.Entry
{
    public class AuthorisationModule
    {
        private IUserSaver _saverService;
        private IScheduler _scheduler;
        private IEmailSender _sender;

        public AuthorisationModule(IUserSaver userSaver, IScheduler scheduler)
        {
            _scheduler = scheduler;
            _saverService = userSaver;
            _sender = new EmailSender();
        }

        public EventResult OnEnter(AuthorisationEventArgs e)
        {
            string email = e.Email;
            string password = e.Password;
            // some checking on server
            User user;
            try
            {
                user = _saverService.GetUser(email);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }

            if (user == null)
            {
                // some actions on unexisting user
                return new EventResult(false, "No such user");
            }
            if (!user.Password.Equals(password))
            {
                // some error actions
                return new EventResult(false, "Incorrect login or password");
            }

            // Load User's scheduler config and place all schedule items on scheduling
            foreach (OnceRunningScheduleItem item in _scheduler.LoadUserConfig(user.Email).Values)
            {
                _scheduler.PlaceOnScheduling(item);
                user.ScheduleItems.Add(item.Id, item);
            }

            EventResult result = new EventResult(true);
            result.OptionalInfo = user;
            return result;
        }

        public EventResult OnForgotPassword(AuthorisationEventArgs e)
        {
            string email = e.Email;
            User user;
            try
            {
                user = _saverService.GetUser(email);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            if (user == null)
            {
                // some actions on unexisting user
                return new EventResult(false, "Unexisting user");
            }
            try
            {
                _sender.Send(email, user.Password);
                return new EventResult(true);
            }
            catch (SmtpException ex)
            {
                string message = "Problems with connection. Can't send email by " + ex.Message;
                Console.WriteLine(message);
                return new EventResult(false, message); // when someone wants to test whole system work without net connection,
                // comment this and move return new EventResult(true); to method end
            }
        }

        /// <summary>
        /// Occurs when user logs out or exits from Daily Helper
        /// </summary>
        /// <param name="e">ExitEventArg contains User</param>
        /// <returns>Successful EventResult if all user local configs are successfuly saved</returns>
        public EventResult OnExited(ExitEventArgs e)
        {
            User user = e.User;
            Guid[] keys = user.ScheduleItems.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                OnceRunningScheduleItem item = user.ScheduleItems[keys[i]];
                if (item.TriggeringTime < DateTime.Now)
                {
                    user.ScheduleItems.Remove(item.Id);
                    continue;
                }
                _scheduler.RemoveFromScheduling(item);
            }
            _scheduler.SaveUserConfig(user);
            return new EventResult(true);
        }
    }
}
