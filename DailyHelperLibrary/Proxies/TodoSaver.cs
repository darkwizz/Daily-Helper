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

        public void RemoveTodoItem(User user, TodoItem item)
        {
            _proxy.RemoveTodoItem(user, item);
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
            public void RemoveTodoItem(User user, TodoItem item)
            {
                Channel.RemoveTodoItem(user.ServiceUser, item.ServiceTodoItem);
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
