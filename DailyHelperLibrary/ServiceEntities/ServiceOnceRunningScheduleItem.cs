using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.ServiceEntities
{
    [DataContract(Name = "OnceRunningScheduleItem", Namespace = "Server/")]
    class ServiceOnceRunningScheduleItem
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public DateTime TriggeringTime { get; set; }
        [DataMember]
        public string ExecutablePath { get; set; }
        [DataMember]
        public string Message { get; set; }

        // here will be virtual method to convert into OnceRunningScheduleItem
        public virtual OnceRunningScheduleItem ScheduleItem
        {
            get
            {
                return new OnceRunningScheduleItem(this);
            }
        }
    }
}
