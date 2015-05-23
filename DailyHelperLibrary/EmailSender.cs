using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary
{
    public class EmailSender
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
            using (MailMessage message = new MailMessage("dailyHelper.notifications@gmail.com", email))
            {
                message.Body = text;
                message.Subject = "no-reply notification";
                _client.Send(message);
                Console.WriteLine("Sending " + text + " to " + email); // logging
            }
        }
    }
}
