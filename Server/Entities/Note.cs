using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    class Note
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string NoteText { get; set; }
    }
}
