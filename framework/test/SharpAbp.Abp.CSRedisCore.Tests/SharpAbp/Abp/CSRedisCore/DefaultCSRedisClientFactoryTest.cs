using Microsoft.Extensions.Logging;
using Moq;
using SharpAbp.Abp.CSRedisCore.TestObjects;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.CSRedisCore
{
    public class DefaultCSRedisClientFactoryTest
    {
        private readonly Mock<ILogger<DefaultCSRedisClientFactory>> _mockLogger;
        public DefaultCSRedisClientFactoryTest()
        {
            _mockLogger = new Mock<ILogger<DefaultCSRedisClientFactory>>();
        }

        [Fact]
        public void Get_Test()
        {
            var mockConfigurationSelector = new Mock<ICSRedisConfigurationSelector>();
            mockConfigurationSelector.Setup(x => x.Get("default")).Returns(new CSRedisConfiguration()
            {
                ConnectionString = "192.168.0.1",
                Mode = RedisMode.Single,
                ReadOnly = true
            });

            mockConfigurationSelector.Setup(x => x.Get("Test2")).Returns(new CSRedisConfiguration()
            {
                ConnectionString = "192.168.0.2",
                Mode = RedisMode.Cluster,
                ReadOnly = false
            });

            CSRedis.CSRedisClient defaultClient = default;
            var mockClientBuilder = new Mock<ICSRedisClientBuilder>();
            mockClientBuilder.Setup(x => x.CreateClient(It.IsAny<CSRedisConfiguration>())).Returns(defaultClient);

            ICSRedisClientFactory redisClientFactory = new DefaultCSRedisClientFactory(_mockLogger.Object, mockConfigurationSelector.Object, mockClientBuilder.Object);

            var client1 = redisClientFactory.Get("default");

            mockConfigurationSelector.Verify(x => x.Get("default"), Times.Once);

            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<CSRedisConfiguration>()), Times.Once);

            var client2 = redisClientFactory.Get<TestClient2>();
            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<CSRedisConfiguration>()), Times.Between(1, 3, Moq.Range.Exclusive));

            mockConfigurationSelector.Verify(x => x.Get("Test2"), Times.Once);

            var client3 = redisClientFactory.Get("Test2");
            mockClientBuilder.Verify(x => x.CreateClient(It.IsAny<CSRedisConfiguration>()), Times.Between(1, 3, Moq.Range.Exclusive));
            mockConfigurationSelector.Verify(x => x.Get("Test2"), Times.Once);


            var clients = redisClientFactory.GetAllClients();
            Assert.Equal(2, clients.Count);

        }

        [Fact]
        public void GetNullConfiguration_Test()
        {
            var mockConfigurationSelector = new Mock<ICSRedisConfigurationSelector>();

            CSRedisConfiguration nullConfiguration = null;
            mockConfigurationSelector.Setup(x => x.Get(It.IsAny<string>())).Returns(nullConfiguration);

            var mockClientBuilder = new Mock<ICSRedisClientBuilder>();

            ICSRedisClientFactory redisClientFactory = new DefaultCSRedisClientFactory(_mockLogger.Object, mockConfigurationSelector.Object, mockClientBuilder.Object);

            Assert.Throws<AbpException>(() =>
            {
                redisClientFactory.Get("default");
            });

        }


    }
}
