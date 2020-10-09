using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Micro.Discovery.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class AbpMicroDiscoveryOptionsTest
    {
        [Fact]
        public void Configure_Test()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("service-configuration.json")
                .Build();

            var testProviderType = typeof(Test1ServiceDiscoveryProvider);

            var options = new AbpMicroDiscoveryOptions();

            options.ProviderNameMappers.SetProvider("test1", testProviderType);

            options.Configure(configuration.GetSection("ServiceDiscovery").GetSection("Services"));

            var configuration1 = options.Configurations.GetConfiguration("micro.service1");
            var configuration2 = options.Configurations.GetConfiguration("micro.service2");
            var configuration3 = options.Configurations.GetConfiguration("micro.service3");
            var configuration4 = options.Configurations.GetConfiguration("micro.service4");

            Assert.Equal(testProviderType, configuration1.ProviderType);
            Assert.Equal(testProviderType, configuration2.ProviderType);
            Assert.Equal(testProviderType, configuration3.ProviderType);
            Assert.Equal(testProviderType, configuration4.ProviderType);
        }
    }
}
