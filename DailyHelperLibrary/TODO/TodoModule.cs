using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;

namespace DailyHelperLibrary.TODO
{
    public class TodoModule
    {
        private ITodoSaver _saverService;

        public TodoModule(ITodoSaver proxy)
        {
            _saverService = proxy;
        }

        public EventResult OnTodoAdded(TodoModuleEventArgs e)
        {
            User user = e.User;
            TodoItem item = e.Todo;
            // add todo item to user TODOs
            user.TodoItems.Add(item.Id, item);
            try
            {
                _saverService.SaveTodoItem(user, item);
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }

        public EventResult OnTodoCompleted(TodoModuleEventArgs e)
        {
            User user = e.User;
            TodoItem item = e.Todo;
            // remove TODO from user TODOs
            user.TodoItems.Remove(item.Id);
            try
            {
                _saverService.RemoveTodoItem(user, item);
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }
    }
}
