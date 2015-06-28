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
        User GetUser(string email, string machineName);
        /// <summary>
        /// Save new user into database. If such user already exists, then throw new <code>Exception</code>
        /// </summary>
        /// <param name="user">User to save in DB</param>
        void SaveUser(User user);
        // Notes
        void SaveNote(User user, Note note);
        void RemoveNote(Note note);
        void UpdateNote(Note note);
        // TODO
        void SaveTodoItem(User user, TodoItem item);
        void RemoveTodoItem(TodoItem item);
        // Social Networks
        void SaveAccountInfo(User user, SocialNetworkAccountInfo info);
        // Scheduler
        void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName);
        void RemoveScheduleItem(OnceRunningScheduleItem item);
    }
}
