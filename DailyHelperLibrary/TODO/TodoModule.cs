using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Faults;
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
            try
            {
                User user = e.User;
                TodoItem item = e.Todo;
                // add todo item to user TODOs
                user.TodoItems.Add(item.Id, item);
                _saverService.SaveTodoItem(user, item);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }

        public EventResult OnTodoCompleted(TodoModuleEventArgs e)
        {
            try
            {
                User user = e.User;
                TodoItem item = e.Todo;
                // remove TODO from user TODOs
                user.TodoItems.Remove(item.Id);
                _saverService.RemoveTodoItem(item);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }
    }
}
