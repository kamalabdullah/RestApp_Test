using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.DataDomain.Configuration
{
    public class AppConfiguration
    {
        public int CountOfRequestRetry { get; set; }
        public int InitialDelayForRetryIntervalInSecond { get; set; }
    }
}
