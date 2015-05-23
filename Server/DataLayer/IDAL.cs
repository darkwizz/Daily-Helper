using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.DataLayer
{
    interface IDAL
    {
        User GetUser(string email);
        void SaveUser(User user);
        void SaveNote(User user, Note note);
        void RemoveNote(User user, Note note);
        void UpdateNote(User user, Note note);
        void SaveTodoItem(User user, TodoItem item);
        void RemoveTodoItem(User user, TodoItem item);
    }
}
