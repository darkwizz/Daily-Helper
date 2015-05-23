using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    public class TodoItem
    {
        internal ServiceTodoItem ServiceTodoItem
        {
            get
            {
                return new ServiceTodoItem { Id = Id, TodoText = TodoText };
            }
        }

        public Guid Id { get; private set; }
        public string TodoText { get; private set; }

        internal TodoItem(ServiceTodoItem item)
        {
            Id = item.Id;
            TodoText = item.TodoText;
        }

        public TodoItem(string todoText)
        {
            Id = Guid.NewGuid();
            TodoText = todoText;
        }
    }
}
