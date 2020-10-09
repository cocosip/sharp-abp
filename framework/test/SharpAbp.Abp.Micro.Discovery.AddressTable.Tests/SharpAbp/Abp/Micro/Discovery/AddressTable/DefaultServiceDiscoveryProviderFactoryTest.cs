using SharpAbp.Abp.Micro.Discovery.AddressTable.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class DefaultServiceDiscoveryProviderFactoryTest : AbpMicroDiscoveryAddressTableTestBase
    {
        private readonly IServiceDiscoveryProviderFactory _serviceDiscoveryProviderFactory;
        public DefaultServiceDiscoveryProviderFactoryTest()
        {
            _serviceDiscoveryProviderFactory = GetRequiredService<IServiceDiscoveryProviderFactory>();
        }

        [Fact]
        public void Get_Test()
        {
            var addressTableProviderType = typeof(AddressTableServiceDiscoveryProvider);
            var test1ServiceDiscoveryProviderType = typeof(Test1ServiceDiscoveryProvider);

            var service1 = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service1");
            Assert.Equal(addressTableProviderType, service1.GetType());

            var service2 = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service2");
            Assert.Equal(addressTableProviderType, service2.GetType());

            var service3= _serviceDiscoveryProviderFactory.Get("micro.addresstable.service3");
            Assert.Equal(test1ServiceDiscoveryProviderType, service3.GetType());

            var service4 = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service4");
            Assert.Equal(addressTableProviderType, service4.GetType());

            var service5 = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service5");
            Assert.Equal(addressTableProviderType, service5.GetType());

        }

    }
}
