using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Savers
{
    public interface ITodoSaver
    {
        void SaveTodoItem(User user, TodoItem item);
        void RemoveTodoItem(User user, TodoItem item);
    }
}
