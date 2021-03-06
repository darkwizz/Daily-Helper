﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.ServiceEntities
{
    [DataContract(Name = "SocialNetworkAccountInfo", Namespace = "Server/")]
    enum ServiceSocialNetworkAccounts
    {
        [EnumMember]
        VK,
        [EnumMember]
        Twitter,
        [EnumMember]
        Facebook
    }

    [DataContract(Namespace = "Server/")]
    class ServiceSocialNetworkAccountInfo
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public ServiceSocialNetworkAccounts ServiceAccount { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

        internal SocialNetworkAccountInfo AccountInfo
        {
            get
            {
                return new SocialNetworkAccountInfo(this);
            }
        }
    }
}
