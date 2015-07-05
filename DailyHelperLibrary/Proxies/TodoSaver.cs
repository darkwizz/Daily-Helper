using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Proxies
{
    public class TodoSaver : ITodoSaver, IDisposable
    {
        private TodoSaverProxy _proxy = new TodoSaverProxy();

        public void SaveTodoItem(User user, TodoItem item)
        {
            _proxy.SaveTodoItem(user, item);
        }

        public void RemoveTodoItem(TodoItem item)
        {
            _proxy.RemoveTodoItem(item);
        }

        public Dictionary<Guid, TodoItem> GetTodoItems(User user)
        {
            return _proxy.GetTodoItems(user);
        }

        public void Dispose()
        {
            _proxy.Close();
        }

        class TodoSaverProxy : ClientBase<ITodoSaverService>
        {
            public TodoSaverProxy() :
                base("SaveTodoEndpoint")
            { }

            public void SaveTodoItem(User user, TodoItem item)
            {
                Channel.SaveTodoItem(user.ServiceUser, item.ServiceTodoItem);
            }

            public void RemoveTodoItem(TodoItem item)
            {
                Channel.RemoveTodoItem(item.ServiceTodoItem);
            }

            public Dictionary<Guid, TodoItem> GetTodoItems(User user)
            {
                return Channel.GetTodoItems(user.ServiceUser).ToDictionary(x => x.Key, x => x.Value.TodoItem);
            }

            new public void Close()
            {
                try
                {
                    base.Close();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Some problems with connection. " + ex.Message); // logging
                }
            }
        }
    }
}
