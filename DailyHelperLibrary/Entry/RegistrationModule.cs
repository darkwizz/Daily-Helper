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
    public class RegistrationModule
    {
        private IUserSaver _saverService;
        private EmailSender _sender;
        private string _checkingKey;

        public RegistrationModule(IUserSaver userSaver)
        {
            _saverService = userSaver;
            _sender = new EmailSender();
        }

        public EventResult OnRegisterUser(RegistrationEventArgs e)
        {
            string email = e.Email;
            _checkingKey = Guid.NewGuid().ToString();

            // here will be some sending to server to check does this email stil isn't registered in DH system

            try
            {
                _sender.Send(email, _checkingKey);
                return new EventResult(true);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Problems with connection. " + ex.Message);
                Console.WriteLine("Sending {0} to {1}", _checkingKey, email);
                return new EventResult(false, "Problems with connection. " + ex.Message);
            }
            // here will be some Exception catching to check on existing email address
            // and throw more high-level exception defined in DH library
        }

        public EventResult OnCheckingCodeAccept(AcceptingCheckingKeyEventArgs e)
        {
            string enteredKey = e.EnteredKey;
            if (!enteredKey.Equals(_checkingKey))
            {
                // here some error exception will be thrown
                return new EventResult(false, "Incorrect checking key");
            }
            string email = e.Email;
            string password = e.Password;
            try
            {
                _saverService.RegisterUser(new User(email, password));
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }
    }
}
