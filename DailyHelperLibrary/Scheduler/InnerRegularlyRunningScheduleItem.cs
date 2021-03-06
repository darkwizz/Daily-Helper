﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Scheduler
{
    [DataContract]
    class InnerRegularlyRunningScheduleItem: InnerOnceRunningScheduleItem
    {
        [DataMember]
        public bool[] RunningDays { get; set; }

        // here will be overriding of convert into OnceRunningScheduleItem method
        public override OnceRunningScheduleItem ScheduleItem
        {
            get
            {
                return new RegularlyRunningScheduleItem(this);
            }
        }
    }
}
