using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    class User
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public Dictionary<Guid, Note> Notes { get; set; }
        [DataMember]
        public Dictionary<Guid, TodoItem> TodoItems { get; set; }

        public User()
        {
            Notes = new Dictionary<Guid, Note>();
            TodoItems = new Dictionary<Guid, TodoItem>();
        }
    }
}
