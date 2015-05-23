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
    class ServiceNote
    {
        public Note Note
        {
            get
            {
                return new Note(this);
            }
        }

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string NoteText { get; set; }
    }
}
