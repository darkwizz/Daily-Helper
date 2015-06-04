using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    [DataContract(Namespace = "Server/")]
    enum SocialNetworkAccounts
    {
        [EnumMember]
        VK,
        [EnumMember]
        Twitter,
        [EnumMember]
        Facebook
    }

    [DataContract(Namespace = "Server/")]
    class SocialNetworkAccountInfo
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public SocialNetworkAccounts Account { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}
