using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Micro.Discovery.AddressTable.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery.AddressTable
{
    public class AbpMicroDiscoveryAddressTableOptionsTest
    {

        [Fact]
        public void Configure_Test()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var options = new AbpMicroDiscoveryOptions();
            options.ProviderNameMappers.SetProvider<AddressTableServiceDiscoveryProvider>("AddressTable");
            options.ProviderNameMappers.SetProvider<Test1ServiceDiscoveryProvider>("test1");

            options.Configure(configuration.GetSection("Services"));

            var defaultProvider = options.Configurations.GetConfiguration(DefaultDiscovery.Name);

            Assert.Equal(typeof(AddressTableServiceDiscoveryProvider), defaultProvider.ProviderType);
        }

    }
}
