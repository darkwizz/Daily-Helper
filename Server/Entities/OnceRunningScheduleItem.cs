using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    class OnceRunningScheduleItem
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public DateTime TriggeringTime { get; set; }
        [DataMember]
        public string ExecutablePath { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
