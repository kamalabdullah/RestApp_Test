using Moq;
using RestApp.DataDomain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.UnitTests
{
    public static class ConfigurationFactory
    {
        public static IConfigurationOptions CreateDEVConf()
        {
            var config = new Mock<IConfigurationOptions>();
            config.Setup(c => c.AppConfiguration).Returns(new AppConfiguration {CountOfRequestRetry =3,InitialDelayForRetryIntervalInSecond =1 });
            return config.Object;
        }
    }
}
