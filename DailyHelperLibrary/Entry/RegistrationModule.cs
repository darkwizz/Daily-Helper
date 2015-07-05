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
using DailyHelperLibrary.Exceptions;
using DailyHelperLibrary.Faults;

namespace DailyHelperLibrary.Entry
{
    public class RegistrationModule
    {
        private IUserSaver _saverService;
        private IEmailSender _sender;
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

            try
            {
                // here will be some sending to server to check does this email stil isn't registered in DH system
                // here will be some Exception catching to check on existing email address
                // and throw more high-level exception defined in DH library
                User user = _saverService.GetUser(email);
                _sender.Send(email, _checkingKey);
                return new EventResult(true);
            }
            catch (FaultException<DataAlreadyExistsFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
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
