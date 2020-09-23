using Consul;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace SharpAbp.Abp.Consul
{
    public class DefaultConsulClientFactoryTest
    {
        private readonly Mock<ILogger<DefaultConsulClientFactory>> _mockLogger;
        public DefaultConsulClientFactoryTest()
        {
            _mockLogger = new Mock<ILogger<DefaultConsulClientFactory>>();
        }

        [Fact]
        public void Get_GetAll_Test()
        {

            var mockConfigurationProvider = new Mock<IConsulConfigurationProvider>();

            var defaultConfiguration = new ConsulConfiguration()
            {
                Address = new Uri("http://127.0.0.1"),
                DataCenter = "DataCenter1",
                Token = "123",
                WaitTime = TimeSpan.FromSeconds(5)
            };

            var configuration1 = new ConsulConfiguration()
            {
                Address = new Uri("http://192.168.0.100/api"),
                DataCenter = "DataCenter2",
                Token = "456",
                WaitTime = null
            };

            var mockConsulClient = new Mock<IConsulClient>();


            mockConfigurationProvider.Setup(x => x.Get("default")).Returns(defaultConfiguration);

            mockConfigurationProvider.Setup(x => x.Get("Test1")).Returns(configuration1);


            var mockClientBuilder = new Mock<IConsulClientBuilder>();
            mockClientBuilder.Setup(x => x.CreateClient(configuration1)).Returns(mockConsulClient.Object);


            var consulClientFactory = new DefaultConsulClientFactory(_mockLogger.Object, mockConfigurationProvider.Object, mockClientBuilder.Object);

            var defaultClient = consulClientFactory.Get();
            mockClientBuilder.Verify(x => x.CreateClient(defaultConfiguration), Times.Once);

            var defaultClient2 = consulClientFactory.Get();
            mockClientBuilder.Verify(x => x.CreateClient(defaultConfiguration), Times.Once);

            var consulClient1 = consulClientFactory.Get("Test1");
            mockClientBuilder.Verify(x => x.CreateClient(configuration1), Times.Once);

            var consulClient1_2 = consulClientFactory.Get("Test1");
            mockClientBuilder.Verify(x => x.CreateClient(configuration1), Times.Once);





        }


    }
}
