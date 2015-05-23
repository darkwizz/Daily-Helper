using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.ServiceEntities
{
    [DataContract(Namespace = "Server/")]
    class ServiceTodoItem
    {
        public TodoItem TodoItem
        {
            get
            {
                return new TodoItem(this);
            }
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string TodoText { get; set; }
    }
}
