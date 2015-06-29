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
        private IUserSaver _userSaverService;
        private IScheduleItemSaver _scheduleItemSaverService;
        private IScheduler _scheduler;
        private IEmailSender _sender;

        public AuthorisationModule(IUserSaver userSaver, IScheduleItemSaver scheduleItemSaver, IScheduler scheduler)
        {
            _scheduler = scheduler;
            _userSaverService = userSaver;
            _scheduleItemSaverService = scheduleItemSaver;
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
                // Load User and especialy User's scheduler config and place all schedule items on scheduling
                // I don't know yet is MachineName unique. If it isn't then it'll need to find
                user = _userSaverService.GetUser(email, Environment.MachineName);
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

            foreach (var item in user.ScheduleItems.Values)
            {
                _scheduler.PlaceOnScheduling(item);
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
                user = _userSaverService.GetUser(email, Environment.MachineName);
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
            foreach (var item in user.ScheduleItems.Values)
            {
                _scheduler.RemoveFromScheduling(item);
                if (item.TriggeringTime < DateTime.Now)
                {
                    _scheduleItemSaverService.DeleteScheduleItem(item);
                }
            }
            return new EventResult(true);
        }
    }
}
