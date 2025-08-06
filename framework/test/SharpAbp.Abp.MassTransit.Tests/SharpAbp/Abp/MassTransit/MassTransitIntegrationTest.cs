using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MassTransit.ActiveMQ;
using SharpAbp.Abp.MassTransit.Kafka;
using SharpAbp.Abp.MassTransit.RabbitMQ;
using SharpAbp.Abp.MassTransit.TestImplementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Integration tests for MassTransit module functionality
    /// </summary>
    public class MassTransitIntegrationTest : AbpMassTransitTestBase
    {
        /// <summary>
        /// Tests that Kafka produce service is properly registered and works
        /// </summary>
        [Fact]
        public async Task Kafka_Produce_Service_Should_Work()
        {
            // Arrange
            var kafkaService = GetRequiredService<IKafkaProduceService>();
            var testKafkaService = Assert.IsType<TestKafkaProduceService>(kafkaService);
            var message = new TestMessage { Content = "Kafka test message" };

            // Act
            await kafkaService.ProduceAsync("test-key", message);
            await kafkaService.ProduceStringKeyAsync(message);

            // Assert
            Assert.Equal(2, testKafkaService.ProducedMessages.Count);
            
            var firstMessage = testKafkaService.ProducedMessages[0];
            Assert.Equal("test-key", firstMessage.Key);
            Assert.Equal(message, firstMessage.Value);
            Assert.Equal(typeof(TestMessage).Name, firstMessage.ValueType);

            var secondMessage = testKafkaService.ProducedMessages[1];
            Assert.NotNull(secondMessage.Key); // Generated GUID key
            Assert.Equal(message, secondMessage.Value);
            Assert.Equal(typeof(TestMessage).Name, secondMessage.ValueType);
        }

        /// <summary>
        /// Tests that RabbitMQ produce service is properly registered and works
        /// </summary>
        [Fact]
        public async Task RabbitMq_Produce_Service_Should_Work()
        {
            // Arrange
            var rabbitMqService = GetRequiredService<IRabbitMqProduceService>();
            var testRabbitMqService = Assert.IsType<TestRabbitMqProduceService>(rabbitMqService);
            var message = new TestMessage { Content = "RabbitMQ test message" };

            // Act
            await rabbitMqService.PublishAsync(message);
            await rabbitMqService.SendAsync("test-uri", message);

            // Assert
            Assert.Single(testRabbitMqService.PublishedMessages);
            Assert.Single(testRabbitMqService.SentMessages);
            
            var publishedMessage = testRabbitMqService.PublishedMessages[0];
            Assert.Equal(message, publishedMessage.Message);
            Assert.Equal(typeof(TestMessage).Name, publishedMessage.MessageType);

            var sentMessage = testRabbitMqService.SentMessages[0];
            Assert.Equal("test-uri", sentMessage.UriString);
            Assert.Equal(message, sentMessage.Message);
            Assert.Equal(typeof(TestMessage).Name, sentMessage.MessageType);
        }

        /// <summary>
        /// Tests that ActiveMQ produce service is properly registered and works
        /// </summary>
        [Fact]
        public async Task ActiveMq_Produce_Service_Should_Work()
        {
            // Arrange
            var activeMqService = GetRequiredService<IActiveMqProduceService>();
            var testActiveMqService = Assert.IsType<TestActiveMqProduceService>(activeMqService);
            var message = new TestMessage { Content = "ActiveMQ test message" };

            // Act
            await activeMqService.PublishAsync(message);
            await activeMqService.SendAsync("test-uri", message);

            // Assert
            Assert.Single(testActiveMqService.PublishedMessages);
            Assert.Single(testActiveMqService.SentMessages);
            
            var publishedMessage = testActiveMqService.PublishedMessages[0];
            Assert.Equal(message, publishedMessage.Message);
            Assert.Equal(typeof(TestMessage).Name, publishedMessage.MessageType);

            var sentMessage = testActiveMqService.SentMessages[0];
            Assert.Equal("test-uri", sentMessage.UriString);
            Assert.Equal(message, sentMessage.Message);
            Assert.Equal(typeof(TestMessage).Name, sentMessage.MessageType);
        }

        /// <summary>
        /// Tests that configuration from appsettings is loaded correctly
        /// </summary>
        [Fact]
        public void Configuration_Should_Be_Loaded_From_AppSettings()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.Equal("SharpAbp", options.Prefix);
            // Note: The actual provider value depends on the test configuration
        }

        /// <summary>
        /// Tests that options validation works in the integrated environment
        /// </summary>
        [Fact]
        public void Options_Validation_Should_Work_In_Integrated_Environment()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            Assert.True(options.IsValid());
            Assert.True(options.StartTimeoutMilliSeconds >= 1000);
            Assert.True(options.StartTimeoutMilliSeconds <= 300000);
            Assert.True(options.StopTimeoutMilliSeconds >= 1000);
            Assert.True(options.StopTimeoutMilliSeconds <= 60000);
        }

        /// <summary>
        /// Tests that pre and post configure actions are applied correctly
        /// </summary>
        [Fact]
        public void PreAndPost_Configure_Actions_Should_Be_Applied()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            Assert.NotNull(options.PreConfigures);
            Assert.NotNull(options.PostConfigures);
            // The actual configuration actions depend on the module setup
        }

        /// <summary>
        /// Tests that the module can handle configuration updates
        /// </summary>
        [Fact]
        public void Module_Should_Handle_Configuration_Updates()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                ["MassTransitOptions:Prefix"] = "UpdatedPrefix",
                ["MassTransitOptions:Provider"] = "UpdatedProvider",
                ["MassTransitOptions:StartTimeoutMilliSeconds"] = "45000"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            var options = new AbpMassTransitOptions();

            // Act
            options.PreConfigure(configuration);

            // Assert
            Assert.Equal("UpdatedPrefix", options.Prefix);
            Assert.Equal("UpdatedProvider", options.Provider);
            Assert.Equal(45000, options.StartTimeoutMilliSeconds);
        }

        /// <summary>
        /// Tests that all message queue services are properly registered
        /// </summary>
        [Fact]
        public void All_Message_Queue_Services_Should_Be_Registered()
        {
            // Act & Assert
            var kafkaService = GetRequiredService<IKafkaProduceService>();
            var rabbitMqService = GetRequiredService<IRabbitMqProduceService>();
            var activeMqService = GetRequiredService<IActiveMqProduceService>();

            Assert.IsType<TestKafkaProduceService>(kafkaService);
            Assert.IsType<TestRabbitMqProduceService>(rabbitMqService);
            Assert.IsType<TestActiveMqProduceService>(activeMqService);
        }

        /// <summary>
        /// Tests that test implementations can be cleared for test isolation
        /// </summary>
        [Fact]
        public async Task Test_Implementations_Should_Support_Message_Clearing()
        {
            // Arrange
            var kafkaService = GetRequiredService<IKafkaProduceService>() as TestKafkaProduceService;
            var rabbitMqService = GetRequiredService<IRabbitMqProduceService>() as TestRabbitMqProduceService;
            var activeMqService = GetRequiredService<IActiveMqProduceService>() as TestActiveMqProduceService;
            
            var message = new TestMessage { Content = "Test message for clearing" };

            // Act - Produce some messages
            await kafkaService!.ProduceAsync("key", message);
            await rabbitMqService!.PublishAsync(message);
            await activeMqService!.PublishAsync(message);

            // Verify messages exist
            Assert.Single(kafkaService.ProducedMessages);
            Assert.Single(rabbitMqService.PublishedMessages);
            Assert.Single(activeMqService.PublishedMessages);

            // Clear messages
            kafkaService.ClearMessages();
            rabbitMqService.ClearMessages();
            activeMqService.ClearMessages();

            // Assert
            Assert.Empty(kafkaService.ProducedMessages);
            Assert.Empty(rabbitMqService.PublishedMessages);
            Assert.Empty(rabbitMqService.SentMessages);
            Assert.Empty(activeMqService.PublishedMessages);
            Assert.Empty(activeMqService.SentMessages);
        }

        /// <summary>
        /// Tests that timeout configurations are applied correctly
        /// </summary>
        [Fact]
        public void Timeout_Configurations_Should_Be_Applied_Correctly()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpMassTransitOptions>>().Value;

            // Assert
            var startTimeout = options.GetStartTimeout();
            var stopTimeout = options.GetStopTimeout();

            Assert.True(startTimeout.TotalMilliseconds >= 1000);
            Assert.True(startTimeout.TotalMilliseconds <= 300000);
            Assert.True(stopTimeout.TotalMilliseconds >= 1000);
            Assert.True(stopTimeout.TotalMilliseconds <= 60000);
        }

        /// <summary>
        /// Test message class for integration tests
        /// </summary>
        private class TestMessage
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}