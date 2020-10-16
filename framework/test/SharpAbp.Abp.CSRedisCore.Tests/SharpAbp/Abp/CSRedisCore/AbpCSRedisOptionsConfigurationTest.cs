using Microsoft.Extensions.Configuration;
using Xunit;

namespace SharpAbp.Abp.CSRedisCore
{
    public class AbpCSRedisOptionsConfigurationTest
    {
        [Fact]
        public void Configure_Test()
        {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .Build();

            var options = new AbpCSRedisOptions()
                .Configure(configuration.GetSection("CSRedisCore"));

            var defaultConfiguration = options.Clients.GetConfiguration<DefaultClient>();
            Assert.Equal(RedisMode.Single, defaultConfiguration.Mode);
            Assert.Equal("127.0.0.1:6379,password=123,prefix=single_", defaultConfiguration.ConnectionString);
            Assert.False(defaultConfiguration.ReadOnly);
            Assert.Empty(defaultConfiguration.Sentinels);

            var sentinelConfiguration = options.Clients.GetConfiguration("client1");
            Assert.Equal(RedisMode.Sentinel, sentinelConfiguration.Mode);
            Assert.Equal("mymaster,password=123,prefix=sentinel_", sentinelConfiguration.ConnectionString);
            Assert.True(sentinelConfiguration.ReadOnly);
            Assert.Equal(3, sentinelConfiguration.Sentinels.Count);
            Assert.Equal("192.169.1.100:26379", sentinelConfiguration.Sentinels[0]);
            Assert.Equal("192.169.1.101:26379", sentinelConfiguration.Sentinels[1]);
            Assert.Equal("192.169.1.102:26379", sentinelConfiguration.Sentinels[2]);

            var clusterConfiguration = options.Clients.GetConfiguration("client2");
            Assert.Equal(RedisMode.Cluster, clusterConfiguration.Mode);
            Assert.Equal("127.0.0.1:6379,password=123,defaultDatabase=0,poolsize=50", clusterConfiguration.ConnectionString);
            Assert.True(clusterConfiguration.ReadOnly);
            Assert.Empty(clusterConfiguration.Sentinels);

        }

    }
}
