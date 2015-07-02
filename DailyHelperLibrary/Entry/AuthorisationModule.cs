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
using DailyHelperLibrary.Faults;
using DailyHelperLibrary.Exceptions;

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
                if (user == null)
                {
                    // some actions on unexisting user
                    return new EventResult(false, "Unexisting user");
                }
                _sender.Send(email, user.Password);
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
            catch (UnavailableMailRecipientException ex)
            {
                Exception inner = ex.InnerException;
                if (inner != null)
                {
                    Console.WriteLine(inner.Message); // logging
                }
                return new EventResult(false, ex.Message);
            }
            catch (MailSenderException ex)
            {
                Exception inner = ex.InnerException;
                if (inner != null)
                {
                    Console.WriteLine(inner.Message); // logging
                }
                return new EventResult(false, ex.Message);
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
