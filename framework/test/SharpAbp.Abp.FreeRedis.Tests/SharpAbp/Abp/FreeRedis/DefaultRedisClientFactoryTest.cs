using FreeRedis;
using Microsoft.Extensions.Logging;
using Moq;
using SharpAbp.Abp.FreeRedis.TestObjects;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.FreeRedis
{
    public class DefaultRedisClientFactoryTest
    {
        private readonly Mock<ILogger<DefaultRedisClientFactory>> _mockLogger;
        public DefaultRedisClientFactoryTest()
        {
            _mockLogger = new Mock<ILogger<DefaultRedisClientFactory>>();
        }

        [Fact]
        public void Get_GetAll_Test()
        {
            var mockConfigurationSelector = new Mock<IRedisConfigurationProvider>();
            mockConfigurationSelector.Setup(x => x.Get("default")).Returns(new FreeRedisConfiguration()
            {
                ConnectionString = "192.168.0.1",
                Mode = RedisMode.Single,
                ReadOnly = true
            });

            mockConfigurationSelector.Setup(x => x.Get("Test2")).Returns(new FreeRedisConfiguration()
            {
                ConnectionString = "192.168.0.2",
                Mode = RedisMode.Cluster,
                ReadOnly = false
            });

            RedisClient defaultClient = default;
            var mockClientBuilder = new Mock<IRedisClientBuilder>();
            mockClientBuilder.Setup(x => x.CreateClient(It.IsAny<FreeRedisConfiguration>())).Returns(defaultClient);

            IRedisClientFactory redisClientFactory = new DefaultRedisClientFactory(_mockLogger.Object, mockConfigurationSelector.Object, mockClientBuilder.Object);

            var client1 = redisClientFactory.Get("default");

            mockConfigurationSelector.Verify(x => x.Get("default"), Times.Once);

            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<FreeRedisConfiguration>()), Times.Once);

            var client2 = redisClientFactory.Get<TestClient2>();
            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<FreeRedisConfiguration>()), Times.Between(1, 3, Moq.Range.Exclusive));

            mockConfigurationSelector.Verify(x => x.Get("Test2"), Times.Once);

            var client3 = redisClientFactory.Get("Test2");
            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<FreeRedisConfiguration>()), Times.Between(1, 3, Moq.Range.Exclusive));
            mockConfigurationSelector.Verify(x => x.Get("Test2"), Times.Once);


            var clients = redisClientFactory.GetAll();
            Assert.Equal(2, clients.Count);

        }

        [Fact]
        public void GetNullConfiguration_Test()
        {
            var mockConfigurationProvider = new Mock<IRedisConfigurationProvider>();

            FreeRedisConfiguration nullConfiguration = null;
            mockConfigurationProvider.Setup(x => x.Get(It.IsAny<string>())).Returns(nullConfiguration);

            var mockClientBuilder = new Mock<IRedisClientBuilder>();

            IRedisClientFactory redisClientFactory = new DefaultRedisClientFactory(_mockLogger.Object, mockConfigurationProvider.Object, mockClientBuilder.Object);

            Assert.Throws<AbpException>(() =>
            {
                redisClientFactory.Get("default");
            });

        }


    }
}
