using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Entry
{
    public class AcceptingCheckingKeyEventArgs: RegistrationEventArgs
    {
        public string Password { get; set; }
        public string EnteredKey { get; set; }

        public AcceptingCheckingKeyEventArgs(string email, string password, string enteredKey) :
            base(email)
        {
            Password = password;
            EnteredKey = enteredKey;
        }
    }
}
