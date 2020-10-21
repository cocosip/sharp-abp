using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery.Consul
{
    public class AbpMicroDiscoveryConsulOptionsTest : AbpMicroDiscoveryConsulTestBase
    {
        private readonly AbpMicroDiscoveryConsulOptions _options;
        public AbpMicroDiscoveryConsulOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpMicroDiscoveryConsulOptions>>().Value;
        }

        [Fact]
        public void AbpMicroDiscoveryConsulOptions_Value_Test()
        {
            Assert.Equal("http", _options.Scheme);
            Assert.Equal("127.0.0.1", _options.Host);
            Assert.Equal(16000, _options.Port);
            Assert.Equal("dc1", _options.DataCenter);
            Assert.Equal("123", _options.Token);
            Assert.Equal(10, _options.WaitSeconds);
            Assert.Equal(1, _options.MaxConsulClient);
            Assert.Equal("consul_", _options.Prefix);
            Assert.Equal(120, _options.Expires);
            Assert.True(_options.EnablePolling);
            Assert.Equal(60, _options.PollingInterval);
        }

    }
}
