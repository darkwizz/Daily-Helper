using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Exceptions
{
    class UnavailableMailRecipientException : MailSenderException
    {
        public UnavailableMailRecipientException() :
            this("Unavailable mail recipient")
        { }

        public UnavailableMailRecipientException(string message) :
            base(message)
        { }

        public UnavailableMailRecipientException(Exception innerException) :
            this("Unavailable mail recipient", innerException)
        { }

        public UnavailableMailRecipientException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
