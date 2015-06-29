using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Exceptions
{
    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException() :
            this("Connection failed")
        { }

        public ConnectionFailedException(string message) :
            base(message)
        { }

        public ConnectionFailedException(Exception innerException) :
            this("Connection failed", innerException)
        { }

        public ConnectionFailedException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
