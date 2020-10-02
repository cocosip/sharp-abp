using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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

            var testProviderType = typeof(Test1ServiceDiscoveryProvider);

            var options = new AbpMicroDiscoveryOptions();
            options.ProviderNameMappers.SetProvider<AddressTableServiceDiscoveryProvider>("AddressTable");
            options.ProviderNameMappers.SetProvider<Test1ServiceDiscoveryProvider>("test1");

            options.Configurations.ConfigureDefault(c =>
            {
                c.UseAddressTable();
            });

            options.Configure(configuration.GetSection("AddressTable"));




        }

    }
}
