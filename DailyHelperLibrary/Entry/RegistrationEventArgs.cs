using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Entry
{
    public class RegistrationEventArgs: EventArgs
    {
        public string Email { get; set; }

        public RegistrationEventArgs(string email)
        {
            Email = email;
        }
    }
}
