using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.Utilities
{
    public interface ILogger
    {
        public void Error(Exception exception);
    }
}
