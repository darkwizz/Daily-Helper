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

namespace DailyHelperLibrary.Entry
{
    public class AuthorisationModule
    {
        private IUserSaver _saverService;
        private EmailSender _sender;

        public AuthorisationModule(IUserSaver userSaver)
        {
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
            }
            if (!user.Password.Equals(password))
            {
                // some error actions
                return new EventResult(false, "Incorrect login or password");
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
    }
}
