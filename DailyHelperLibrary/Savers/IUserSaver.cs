using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Savers
{
    public interface IUserSaver
    {
        void RegisterUser(User user);
        User GetUser(string email);
    }
}
