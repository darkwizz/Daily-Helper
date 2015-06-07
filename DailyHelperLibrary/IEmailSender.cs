using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary
{
    public interface IEmailSender
    {
        void Send(string email, string text);
    }
}
