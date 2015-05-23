using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.DataLayer
{
    class DALStub: IDAL
    {
        public User GetUser(string email)
        {
            Console.WriteLine("Getting user with email " + email);
            User user = new User { Email = "fake@mail.com", Password = "fake_pwd" };
            return user;
        }

        public void SaveUser(User user)
        {
            Console.WriteLine("User has been saved into DB");
        }

        public void SaveNote(User user, Note note)
        {
            Console.WriteLine("Note of user {0} has been saved", user.Email);
        }

        public void RemoveNote(User user, Note note)
        {
            Console.WriteLine("Note of user {0} has been removed", user.Email);
        }

        public void UpdateNote(User user, Note note)
        {
            Console.WriteLine("Note of user {0} has been updated", user.Email);
        }

        public void SaveTodoItem(User user, TodoItem item)
        {
            Console.WriteLine("TODO item of user {0} has been saved", user.Email);
        }

        public void RemoveTodoItem(User user, TodoItem item)
        {
            Console.WriteLine("TODO item of user {0} has been removed", user.Email);
        }
    }
}
