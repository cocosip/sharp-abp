using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceDiscoveryProviderTest : AbpMicroDiscoveryAddressTableTestBase
    {
        private readonly IServiceDiscoveryProviderFactory _serviceDiscoveryProviderFactory;
        public AddressTableServiceDiscoveryProviderTest()
        {
            _serviceDiscoveryProviderFactory = GetRequiredService<IServiceDiscoveryProviderFactory>();
        }

        [Fact]
        public async Task GetService_Test()
        {
            var provider1 = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service1");
            var services = await provider1.GetAsync("micro.addresstable.service1");

            Assert.Equal(2, services.Count);

            var service_id_1 = services.Find(x => x.Id == "1");
            Assert.Equal("192.168.0.100", service_id_1.Host);
            Assert.Equal(10000, service_id_1.Port);
            Assert.Equal("http", service_id_1.Scheme);
            Assert.Equal("grpc", service_id_1.Tags[0]);


            var service_id_2 = services.Find(x => x.Id == "2");
            Assert.Equal("192.168.0.101", service_id_2.Host);
            Assert.Equal(10001, service_id_2.Port);
            Assert.Equal("http", service_id_2.Scheme);
            Assert.Equal("grpc", service_id_2.Tags[0]);

        }

        [Fact]
        public async Task Empty_Service_Test()
        {
            var provider = _serviceDiscoveryProviderFactory.Get("micro.addresstable.service5");
            Assert.Equal(typeof(AddressTableServiceDiscoveryProvider), provider.GetType());

            var services = await provider.GetAsync("micro.addresstable.service5");
            Assert.Empty(services);
        }



    }
}
