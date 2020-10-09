using SharpAbp.Abp.Micro.Discovery.TestObjects;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class DefaultServiceDiscoveryProviderFactoryTest : AbpMicroDiscoveryTestBase
    {
        private readonly IServiceDiscoveryProviderFactory _serviceDiscoveryProviderFactory;
        public DefaultServiceDiscoveryProviderFactoryTest()
        {
            _serviceDiscoveryProviderFactory = GetRequiredService<IServiceDiscoveryProviderFactory>();
        }

        [Fact]
        public void Get_Configured_Service_Provider_Test()
        {
            var provider1 = _serviceDiscoveryProviderFactory.Get("micro.test.service1");
            Assert.Equal(typeof(Test1ServiceDiscoveryProvider), provider1.GetType());

            var provider2 = _serviceDiscoveryProviderFactory.Get("micro.test.service2");
            Assert.Equal(typeof(Test2ServiceDiscoveryProvider), provider2.GetType());

            //Default provider
            var getDefaultProvider = _serviceDiscoveryProviderFactory.Get("micro.test.11");
            Assert.Equal(typeof(Test1ServiceDiscoveryProvider), getDefaultProvider.GetType());

            Assert.Throws<AbpException>(() =>
            {
                _serviceDiscoveryProviderFactory.Get("micro.test.service3");
            });
        }

    }
}
