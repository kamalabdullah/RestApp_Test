using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.DataDomain.Configuration
{
    public class ConfigurationOptions : IConfigurationOptions
    {
        /// <summary>
        /// Gets or sets the Application Configrations.
        /// </summary>
        /// <value>
        /// The  Application Configrations.
        /// </value>
        public AppConfiguration AppConfiguration { get; set; }
    }
}
