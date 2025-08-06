using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Unit tests for <see cref="AbpMassTransitOptions"/>
    /// </summary>
    public class AbpMassTransitOptionsTest
    {
        /// <summary>
        /// Tests that the constructor initializes properties correctly
        /// </summary>
        [Fact]
        public void Constructor_Should_Initialize_Properties_Correctly()
        {
            // Act
            var options = new AbpMassTransitOptions();

            // Assert
            Assert.NotNull(options.PreConfigures);
            Assert.NotNull(options.PostConfigures);
            Assert.Empty(options.PreConfigures);
            Assert.Empty(options.PostConfigures);
            Assert.True(options.WaitUntilStarted);
            Assert.Equal(30000, options.StartTimeoutMilliSeconds);
            Assert.Equal(10000, options.StopTimeoutMilliSeconds);
            Assert.Null(options.Prefix);
            Assert.Null(options.Provider);
        }

        /// <summary>
        /// Tests that IsValid returns true for valid configuration
        /// </summary>
        [Fact]
        public void IsValid_Should_Return_True_For_Valid_Configuration()
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = 30000,
                StopTimeoutMilliSeconds = 10000
            };

            // Act & Assert
            Assert.True(options.IsValid());
        }

        /// <summary>
        /// Tests that IsValid returns false for invalid start timeout
        /// </summary>
        [Theory]
        [InlineData(500)]    // Too low
        [InlineData(400000)] // Too high
        public void IsValid_Should_Return_False_For_Invalid_StartTimeout(int startTimeout)
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = startTimeout,
                StopTimeoutMilliSeconds = 10000
            };

            // Act & Assert
            Assert.False(options.IsValid());
        }

        /// <summary>
        /// Tests that IsValid returns false for invalid stop timeout
        /// </summary>
        [Theory]
        [InlineData(500)]   // Too low
        [InlineData(70000)] // Too high
        public void IsValid_Should_Return_False_For_Invalid_StopTimeout(int stopTimeout)
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = 30000,
                StopTimeoutMilliSeconds = stopTimeout
            };

            // Act & Assert
            Assert.False(options.IsValid());
        }

        /// <summary>
        /// Tests that GetStartTimeout returns correct TimeSpan
        /// </summary>
        [Fact]
        public void GetStartTimeout_Should_Return_Correct_TimeSpan()
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = 45000
            };

            // Act
            var timeout = options.GetStartTimeout();

            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(45000), timeout);
        }

        /// <summary>
        /// Tests that GetStopTimeout returns correct TimeSpan
        /// </summary>
        [Fact]
        public void GetStopTimeout_Should_Return_Correct_TimeSpan()
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StopTimeoutMilliSeconds = 15000
            };

            // Act
            var timeout = options.GetStopTimeout();

            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(15000), timeout);
        }

        /// <summary>
        /// Tests that PreConfigure throws ArgumentNullException for null configuration
        /// </summary>
        [Fact]
        public void PreConfigure_Should_Throw_ArgumentNullException_For_Null_Configuration()
        {
            // Arrange
            var options = new AbpMassTransitOptions();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => options.PreConfigure(null!));
        }

        /// <summary>
        /// Tests that PreConfigure updates properties from configuration
        /// </summary>
        [Fact]
        public void PreConfigure_Should_Update_Properties_From_Configuration()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                ["MassTransitOptions:Prefix"] = "TestPrefix",
                ["MassTransitOptions:Provider"] = "RabbitMQ",
                ["MassTransitOptions:WaitUntilStarted"] = "false",
                ["MassTransitOptions:StartTimeoutMilliSeconds"] = "60000",
                ["MassTransitOptions:StopTimeoutMilliSeconds"] = "20000"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            var options = new AbpMassTransitOptions();

            // Act
            var result = options.PreConfigure(configuration);

            // Assert
            Assert.Same(options, result); // Should return same instance for chaining
            Assert.Equal("TestPrefix", options.Prefix);
            Assert.Equal("RabbitMQ", options.Provider);
            Assert.False(options.WaitUntilStarted);
            Assert.Equal(60000, options.StartTimeoutMilliSeconds);
            Assert.Equal(20000, options.StopTimeoutMilliSeconds);
        }

        /// <summary>
        /// Tests that PreConfigure handles missing configuration section gracefully
        /// </summary>
        [Fact]
        public void PreConfigure_Should_Handle_Missing_Configuration_Section_Gracefully()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();
            var options = new AbpMassTransitOptions
            {
                Prefix = "OriginalPrefix",
                Provider = "OriginalProvider"
            };

            // Act
            var result = options.PreConfigure(configuration);

            // Assert
            Assert.Same(options, result);
            // Original values should be preserved when configuration section is missing
            Assert.Equal("OriginalPrefix", options.Prefix);
            Assert.Equal("OriginalProvider", options.Provider);
        }

        /// <summary>
        /// Tests that PreConfigure handles partial configuration correctly
        /// </summary>
        [Fact]
        public void PreConfigure_Should_Handle_Partial_Configuration_Correctly()
        {
            // Arrange
            var configurationData = new Dictionary<string, string>
            {
                ["MassTransitOptions:Prefix"] = "NewPrefix"
                // Only setting Prefix, other properties should use defaults from configuration binding
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            var options = new AbpMassTransitOptions
            {
                Provider = "OriginalProvider",
                StartTimeoutMilliSeconds = 45000
            };

            // Act
            options.PreConfigure(configuration);

            // Assert
            Assert.Equal("NewPrefix", options.Prefix);
            // These should be updated to defaults since they're not in config and Get<T>() returns new instance
            Assert.Null(options.Provider);
            Assert.Equal(30000, options.StartTimeoutMilliSeconds); // Default value
        }
    }
}