using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Savers
{
    public interface ISocialNetworkAccountInfoSaver
    {
        void UpdateInfo(User user, SocialNetworkAccountInfo info);
    }
}
