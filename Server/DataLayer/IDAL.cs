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
        // User
        User GetUser(string email);
        void SaveUser(User user);
        // Notes
        void SaveNote(User user, Note note);
        void RemoveNote(User user, Note note);
        void UpdateNote(User user, Note note);
        // TODO
        void SaveTodoItem(User user, TodoItem item);
        void RemoveTodoItem(User user, TodoItem item);
        // Social Networks
        void SaveAccountInfo(User user, SocialNetworkAccountInfo info);
    }
}
