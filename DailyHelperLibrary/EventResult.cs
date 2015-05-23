using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary
{
    public class EventResult
    {
        public bool IsSuccessful { get; private set; }
        public string ErrorDescription { get; private set; }
        public object OptionalInfo { get; set; }

        public EventResult(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }

        public EventResult(bool isSuccessful, string errorDescription) :
            this(isSuccessful)
        {
            ErrorDescription = errorDescription;
        }
    }
}
