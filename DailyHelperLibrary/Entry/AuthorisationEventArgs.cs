using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyHelperLibrary.Entry
{
    public class AuthorisationEventArgs: EventArgs
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public AuthorisationEventArgs(string email, string password = "")
        {
            Email = email;
            Password = password;
        }
    }
}
