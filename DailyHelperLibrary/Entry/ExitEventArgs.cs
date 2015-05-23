using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Entry
{
    public class ExitEventArgs
    {
        public User User { get; private set; }

        public ExitEventArgs(User user)
        {
            User = user;
        }
    }
}
