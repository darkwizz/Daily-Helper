using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Exceptions
{
    public class MailSenderException: Exception
    {
        public MailSenderException() :
            this("Can't send email")
        { }

        public MailSenderException(string message) :
            base(message)
        { }

        public MailSenderException(Exception innerException) :
            this("Can't send email", innerException)
        { }

        public MailSenderException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
