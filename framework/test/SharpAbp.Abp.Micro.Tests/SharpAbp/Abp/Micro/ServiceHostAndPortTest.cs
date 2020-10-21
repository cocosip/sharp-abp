using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.Micro
{
    public class ServiceHostAndPortTest
    {
        [Fact]
        public void ServiceHostAndPort_Equal_Test()
        {
            var serviceHostAndPort1 = new ServiceHostAndPort("127.0.0.100", 12000);
            var serviceHostAndPort2 = new ServiceHostAndPort("127.0.0.100", 12000);
            var serviceHostAndPort3 = new ServiceHostAndPort("127.0.0.101", 12001);
            Assert.True(serviceHostAndPort1.Equals(serviceHostAndPort2));
            Assert.True(serviceHostAndPort1 == serviceHostAndPort2);

            var list = new List<ServiceHostAndPort>()
            {
                serviceHostAndPort1,
                serviceHostAndPort2,
                serviceHostAndPort3
            };

            Assert.Contains(new ServiceHostAndPort("127.0.0.101", 12001), list);

        }

    }
}
