using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    class TodoItem
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string TodoText { get; set; }
    }
}
