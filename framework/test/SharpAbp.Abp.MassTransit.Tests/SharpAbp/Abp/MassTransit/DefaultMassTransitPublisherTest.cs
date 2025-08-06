using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Unit tests for <see cref="DefaultMassTransitPublisher"/>
    /// </summary>
    public class DefaultMassTransitPublisherTest
    {
        private readonly Mock<IOptionsSnapshot<AbpMassTransitOptions>> _optionsMock;
        private readonly Mock<ILogger<DefaultMassTransitPublisher>> _loggerMock;
        private readonly AbpMassTransitOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMassTransitPublisherTest"/> class
        /// </summary>
        public DefaultMassTransitPublisherTest()
        {
            _optionsMock = new Mock<IOptionsSnapshot<AbpMassTransitOptions>>();
            _loggerMock = new Mock<ILogger<DefaultMassTransitPublisher>>();
            
            _options = new AbpMassTransitOptions
            {
                Provider = "RabbitMQ"
            };
            
            _optionsMock.Setup(x => x.Value).Returns(_options);
        }

        /// <summary>
        /// Creates a new ServiceCollection for each test to avoid service registration conflicts
        /// </summary>
        /// <returns>A new ServiceCollection instance</returns>
        private ServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <summary>
        /// Tests that constructor throws ArgumentNullException for null options
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_For_Null_Options()
        {
            // Arrange
            var services = CreateServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DefaultMassTransitPublisher(null!, serviceProvider, _loggerMock.Object));
        }

        /// <summary>
        /// Tests that constructor throws ArgumentNullException for null service provider
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_For_Null_ServiceProvider()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DefaultMassTransitPublisher(_optionsMock.Object, null!, _loggerMock.Object));
        }

        /// <summary>
        /// Tests that constructor throws ArgumentNullException for null logger
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_For_Null_Logger()
        {
            // Arrange
            var services = CreateServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, null!));
        }

        /// <summary>
        /// Tests that PublishAsync throws ArgumentNullException for null message
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Throw_ArgumentNullException_For_Null_Message()
        {
            // Arrange
            var services = CreateServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                publisher.PublishAsync<object>(null!, CancellationToken.None));
        }

        /// <summary>
        /// Tests that PublishAsync throws InvalidOperationException when Provider is null or empty
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task PublishAsync_Should_Throw_InvalidOperationException_For_Invalid_Provider(string? provider)
        {
            // Arrange
            _options.Provider = provider;
            var services = CreateServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);
            var message = new TestMessage { Content = "Test" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                publisher.PublishAsync(message, CancellationToken.None));
            
            Assert.Contains("Provider", exception.Message);
        }

        /// <summary>
        /// Tests that PublishAsync throws InvalidOperationException when publish provider is not found
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Throw_InvalidOperationException_When_Provider_Not_Found()
        {
            // Arrange
            // Don't register any keyed services, so GetRequiredKeyedService will throw
            var services = CreateServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);
            var message = new TestMessage { Content = "Test" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                publisher.PublishAsync(message, CancellationToken.None));
            
            Assert.Contains("RabbitMQ", exception.Message);
            Assert.Contains("not registered", exception.Message);
        }

        /// <summary>
        /// Tests that PublishAsync successfully publishes message when provider is found
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Successfully_Publish_Message_When_Provider_Found()
        {
            // Arrange
            var publishProviderMock = new Mock<IPublishProvider>();
            publishProviderMock.Setup(x => x.Provider).Returns("RabbitMQ");
            publishProviderMock.Setup(x => x.PublishAsync(It.IsAny<TestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Register the mock as a keyed service
            var services = CreateServiceCollection();
            services.AddKeyedSingleton<IPublishProvider>("RabbitMQ", publishProviderMock.Object);
            var serviceProvider = services.BuildServiceProvider();
            
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);
            var message = new TestMessage { Content = "Test" };

            // Act
            await publisher.PublishAsync(message, CancellationToken.None);

            // Assert
            publishProviderMock.Verify(x => x.PublishAsync(message, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that PublishAsync logs and rethrows exceptions from publish provider
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Log_And_Rethrow_Exceptions_From_Provider()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Provider error");
            var publishProviderMock = new Mock<IPublishProvider>();
            publishProviderMock.Setup(x => x.Provider).Returns("RabbitMQ");
            publishProviderMock.Setup(x => x.PublishAsync(It.IsAny<TestMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Register the mock as a keyed service
            var services = CreateServiceCollection();
            services.AddKeyedSingleton<IPublishProvider>("RabbitMQ", publishProviderMock.Object);
            var serviceProvider = services.BuildServiceProvider();
            
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);
            var message = new TestMessage { Content = "Test" };

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                publisher.PublishAsync(message, CancellationToken.None));
            
            Assert.Same(expectedException, actualException);
            
            // Verify logging occurred
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to publish message of type TestMessage using provider RabbitMQ")),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that PublishAsync handles cancellation token correctly
        /// </summary>
        [Fact]
        public async Task PublishAsync_Should_Handle_CancellationToken_Correctly()
        {
            // Arrange
            var publishProviderMock = new Mock<IPublishProvider>();
            publishProviderMock.Setup(x => x.Provider).Returns("RabbitMQ");
            
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            
            publishProviderMock.Setup(x => x.PublishAsync(It.IsAny<TestMessage>(), It.IsAny<CancellationToken>()))
                .Returns((TestMessage msg, CancellationToken ct) => 
                {
                    ct.ThrowIfCancellationRequested();
                    return Task.CompletedTask;
                });

            // Register the mock as a keyed service
            var services = CreateServiceCollection();
            services.AddKeyedSingleton<IPublishProvider>("RabbitMQ", publishProviderMock.Object);
            var serviceProvider = services.BuildServiceProvider();
            
            var publisher = new DefaultMassTransitPublisher(_optionsMock.Object, serviceProvider, _loggerMock.Object);
            var message = new TestMessage { Content = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => 
                publisher.PublishAsync(message, cancellationTokenSource.Token));
        }

        /// <summary>
        /// Test message class for unit tests
        /// </summary>
        private class TestMessage
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}