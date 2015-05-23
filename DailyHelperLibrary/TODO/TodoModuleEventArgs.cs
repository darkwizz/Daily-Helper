using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.TODO
{
    public class TodoModuleEventArgs: EventArgs
    {
        public User User { get; private set; }
        public TodoItem Todo { get; private set; }

        public TodoModuleEventArgs(User user, TodoItem item)
        {
            User = user;
            Todo = item;
        }
    }
}
