using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AddressTableServiceDiscoveryProviderTest : AbpMicroDiscoveryAddressTableTestBase
    {
        private readonly IServiceDiscoveryProvider _serviceDiscoveryProvider;
        public AddressTableServiceDiscoveryProviderTest()
        {
            _serviceDiscoveryProvider = GetRequiredService<IServiceDiscoveryProvider>();
        }

        [Fact]
        public async Task GetServices_Test()
        {

            var services1 = await _serviceDiscoveryProvider.GetServices("micro.addresstable.service1");
            Assert.Equal(2, services1.Count);

            var service_1_id1 = services1.FirstOrDefault(x => x.Id == "1");
            Assert.Equal("192.168.0.100", service_1_id1.Address);
            Assert.Equal(10000, service_1_id1.Port);
            Assert.Equal("grpc", service_1_id1.Tags[0]);

            var services2 = await _serviceDiscoveryProvider.GetServices("micro.addresstable.service2");

            Assert.Single(services2);

            var service_2_id3 = services2.FirstOrDefault();
            Assert.Equal("192.168.0.102", service_2_id3.Address);
            Assert.Equal(12000, service_2_id3.Port);
            Assert.Empty(service_2_id3.Tags);
        }

        [Fact]
        public async Task Get_Empty_Services_Test()
        {
            var services = await _serviceDiscoveryProvider.GetServices("micro.addresstable.service3");
            Assert.Empty(services);
        }

        [Fact]
        public async Task Get_Filter_Service_Test()
        {
            var services1_1 = await _serviceDiscoveryProvider.GetServices("micro.addresstable.service1", "grpc");
            Assert.Equal(2, services1_1.Count);

            var services1_2 = await _serviceDiscoveryProvider.GetServices("micro.addresstable.service1", "http");
            Assert.Single(services1_2);

            var services1_2_2 = services1_2.FirstOrDefault();
            Assert.Equal("2", services1_2_2.Id);
            Assert.Equal("192.168.0.101", services1_2_2.Address);
            Assert.Equal(10001, services1_2_2.Port);
            Assert.Equal(2, services1_2_2.Tags.Count);
            Assert.Equal("grpc", services1_2_2.Tags[0]);
            Assert.Equal("http", services1_2_2.Tags[1]);
        }
    }
}
