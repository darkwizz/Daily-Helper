using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    public class User
    {
        internal ServiceUser ServiceUser
        {
            get
            {
                ServiceUser user = new ServiceUser
                {
                    Id = Id,
                    Email = Email,
                    Password = Password
                };

                Dictionary<Guid, ServiceNote> serviceNotes = new Dictionary<Guid, ServiceNote>();
                foreach (var note in Notes.Values)
                {
                    serviceNotes.Add(note.Id, note.ServiceNote);
                }
                Dictionary<Guid, ServiceTodoItem> serviceTodoItems = new Dictionary<Guid, ServiceTodoItem>();
                foreach (var item in TodoItems.Values)
                {
                    serviceTodoItems.Add(item.Id, item.ServiceTodoItem);
                }

                user.Notes = serviceNotes;
                user.TodoItems = serviceTodoItems;
                return user;
            }
        }

        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; set; }
        public Dictionary<Guid, Note> Notes { get; private set; }
        public Dictionary<Guid, TodoItem> TodoItems { get; private set; }

        internal User(ServiceUser user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;

            Dictionary<Guid, Note> notes = new Dictionary<Guid, Note>();
            foreach (var note in user.Notes.Values)
            {
                Note newNote = note.Note;
                notes.Add(newNote.Id, newNote);
            }
            Dictionary<Guid, TodoItem> todoItems = new Dictionary<Guid, TodoItem>();
            foreach (var item in user.TodoItems.Values)
            {
                TodoItem newItem = item.TodoItem;
                todoItems.Add(newItem.Id, newItem);
            }

            Notes = notes;
            TodoItems = todoItems;
        }

        public User(string email, string password)
        {
            email = Email;
            password = Password;
            Id = Guid.NewGuid();
            Notes = new Dictionary<Guid, Note>();
            TodoItems = new Dictionary<Guid, TodoItem>();
        }
    }
}
