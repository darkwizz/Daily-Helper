using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    class RegularlyRunningScheduleItem: OnceRunningScheduleItem
    {
        [DataMember]
        public bool[] RunningDays { get; set; }
    }
}
