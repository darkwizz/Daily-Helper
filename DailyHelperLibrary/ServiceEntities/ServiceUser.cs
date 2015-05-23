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
    class ServiceUser
    {
        public User User
        {
            get
            {
                return new User(this);
            }
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public Dictionary<Guid, ServiceNote> Notes { get; set; }
        [DataMember]
        public Dictionary<Guid, ServiceTodoItem> TodoItems { get; set; }

        public ServiceUser()
        {
            Notes = new Dictionary<Guid,ServiceNote>();
            TodoItems = new Dictionary<Guid,ServiceTodoItem>();
        }
    }
}
