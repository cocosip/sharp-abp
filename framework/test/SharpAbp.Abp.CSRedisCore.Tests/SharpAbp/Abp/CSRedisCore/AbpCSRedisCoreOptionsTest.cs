using SharpAbp.Abp.CSRedisCore.TestObjects;
using Xunit;

namespace SharpAbp.Abp.CSRedisCore
{
    public class AbpCSRedisCoreOptionsTest : AbpCSRedisCoreTestBase
    {
        private readonly ICSRedisConfigurationSelector _configurationSelector;

        public AbpCSRedisCoreOptionsTest()
        {
            _configurationSelector = GetRequiredService<ICSRedisConfigurationSelector>();
        }

        [Fact]
        public void Get_Configuration_Test()
        {
            var defaultConfiguration = _configurationSelector.Get("default");
            Assert.Equal(RedisMode.Single, defaultConfiguration.Mode);
            Assert.Equal("192.168.0.1", defaultConfiguration.ConnectionString);
            Assert.True(defaultConfiguration.ReadOnly);
            Assert.Empty(defaultConfiguration.Sentinels);

            var configuration1 = _configurationSelector.Get(typeof(TestClient1).FullName);
            Assert.Equal(RedisMode.Sentinel, configuration1.Mode);
            Assert.Equal("127.0.0.1", configuration1.ConnectionString);
            Assert.True(configuration1.ReadOnly);
            Assert.Single(configuration1.Sentinels);
            Assert.Contains("192.168.0.100", configuration1.Sentinels);

            var configuration2 = _configurationSelector.Get("Test2");
            Assert.Equal(RedisMode.Cluster, configuration2.Mode);
            Assert.Equal("127.0.0.2", configuration2.ConnectionString);
            Assert.False(configuration2.ReadOnly);
            Assert.Empty(configuration2.Sentinels);
        }
    }
}
