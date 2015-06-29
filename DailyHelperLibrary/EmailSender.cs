using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Exceptions;

namespace DailyHelperLibrary
{
    public class EmailSender: IEmailSender, IDisposable
    {
        private SmtpClient _client;

        public EmailSender()
        {
            _client = new SmtpClient("smtp.gmail.com", 587);
            _client.EnableSsl = true;
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;
            _client.Credentials = new NetworkCredential("dailyHelper.notifications", "DHwithoutIPM1");
        }

        public void Send(string email, string text)
        {
            try
            {
                using (MailMessage message = new MailMessage("dailyHelper.notifications@gmail.com", email))
                {
                    message.Body = text;
                    message.Subject = "no-reply notification";
                    _client.Send(message);
                    Console.WriteLine("Sending " + text + " to " + email); // logging
                }
            }
            catch (SmtpException ex)
            {
                switch (ex.StatusCode)
                {
                    case SmtpStatusCode.MailboxUnavailable:
                        throw new UnavailableMailRecipientException("Can't send mail message. Unavailable or unexisting mail address", ex);
                    case SmtpStatusCode.ServiceNotAvailable:
                        throw new ConnectionFailedException("Can't send mail message. Internet connection has been failed", ex);
                    default:
                        throw new MailSenderException(ex);
                }
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
