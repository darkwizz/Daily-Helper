﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Faults
{
    [DataContract(Namespace = "Server/")]
    class DataAlreadyExistsFault
    {
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string FullDescription { get; set; }
        [DataMember]
        public Exception InnerException { get; set; }
    }
}