using SharpAbp.Abp.Micro.Discovery.AddressTable;
using SharpAbp.Abp.Micro.Discovery.TestObjects;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class ServiceDiscoveryFactoryTest : AbpMicroDiscoveryTestBase
    {
        private readonly IServiceDiscoveryFactory _serviceDiscoveryFactory;

        public ServiceDiscoveryFactoryTest()
        {
            _serviceDiscoveryFactory = GetRequiredService<IServiceDiscoveryFactory>();
        }


        [Fact]
        public void GetDefaultDiscovery_Test()
        {
            var defaultDiscoverer = _serviceDiscoveryFactory.Create("default");

            var configuration = defaultDiscoverer.GetConfiguration();

            var addressTableDiscoveryConfiguration = configuration.GetAddressTableDiscoveryConfiguration();

            Assert.True(addressTableDiscoveryConfiguration.OverrideException);

        }

        [Fact]
        public async Task Get_Configure_Discovery_Test()
        {
            var discoverer1 = _serviceDiscoveryFactory.Create<TestServiceDiscoverer1>();

            var configuration = discoverer1.GetConfiguration();

            var addressTableDiscoveryConfiguration = configuration.GetAddressTableDiscoveryConfiguration();

            //Why OverrideException is true ???
            //UseAddressTableDiscovery doesn't set any property,so get the property value from default configuration

            Assert.True(addressTableDiscoveryConfiguration.OverrideException);

            var microServices = await discoverer1.GetAsync();

            Assert.Equal(2, microServices.Count);

            var microService1 = microServices.FirstOrDefault(x => x.ID == "1");
            Assert.NotNull(microService1);

            Assert.Equal("192.168.0.100", microService1.Address);
            Assert.Equal(10000, microService1.Port);
        }


    }
}
