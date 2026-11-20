using System;
using FreeRedis;
using Xunit;

namespace SharpAbp.Abp.FreeRedis;

public class RedisClientBuilderLifecycleTest
{
    [Fact]
    public void Failed_Configuration_Releases_The_Partial_Client()
    {
        RedisClient client = null;
        var failure = new InvalidOperationException("configuration failed");
        var configuration = new FreeRedisConfiguration { ConnectionString = "127.0.0.1:1,connectTimeout=100" };
        configuration.CliConfigures.Add(value =>
        {
            client = value;
            throw failure;
        });
        try
        {
            Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => new DefaultRedisClientBuilder().CreateClient(configuration)));
            Assert.NotNull(client);
            var disposed = Assert.Throws<Exception>(() => client.Ping());
            Assert.Contains("IdleBus", disposed.Message);
        }
        finally
        {
            client?.Dispose();
        }
    }
}
