using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Unit tests for <see cref="MassTransitSetupUtil"/>
    /// </summary>
    public class MassTransitSetupUtilTest
    {
        /// <summary>
        /// Tests that ConfigureMassTransitHost throws ArgumentNullException for null context
        /// </summary>
        [Fact]
        public void ConfigureMassTransitHost_Should_Throw_ArgumentNullException_For_Null_Context()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                MassTransitSetupUtil.ConfigureMassTransitHost(null!));
        }

        /// <summary>
        /// Tests that AbpMassTransitOptions validation works correctly
        /// </summary>
        [Fact]
        public void AbpMassTransitOptions_Validation_Should_Work_Correctly()
        {
            // Arrange
            var validOptions = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = 30000,
                StopTimeoutMilliSeconds = 10000
            };

            var invalidOptions = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = 500 // Invalid - too low
            };

            // Act & Assert
            Assert.True(validOptions.IsValid());
            Assert.False(invalidOptions.IsValid());
        }

        /// <summary>
        /// Tests that timeout configurations are applied correctly
        /// </summary>
        [Theory]
        [InlineData(1000, 1000)]   // Minimum valid values
        [InlineData(300000, 60000)] // Maximum valid values
        [InlineData(30000, 10000)]  // Default values
        public void Timeout_Configurations_Should_Be_Applied_Correctly(int startTimeout, int stopTimeout)
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = startTimeout,
                StopTimeoutMilliSeconds = stopTimeout
            };

            // Act
            var startTimeSpan = options.GetStartTimeout();
            var stopTimeSpan = options.GetStopTimeout();

            // Assert
            Assert.Equal(TimeSpan.FromMilliseconds(startTimeout), startTimeSpan);
            Assert.Equal(TimeSpan.FromMilliseconds(stopTimeout), stopTimeSpan);
            Assert.True(options.IsValid());
        }

        /// <summary>
        /// Tests that WaitUntilStarted flag works correctly
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WaitUntilStarted_Flag_Should_Work_Correctly(bool waitUntilStarted)
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                WaitUntilStarted = waitUntilStarted,
                StartTimeoutMilliSeconds = 30000,
                StopTimeoutMilliSeconds = 10000
            };

            // Act & Assert
            Assert.Equal(waitUntilStarted, options.WaitUntilStarted);
            Assert.True(options.IsValid());
        }

        /// <summary>
        /// Tests that invalid timeout values are properly detected
        /// </summary>
        [Theory]
        [InlineData(500, 10000)]    // Start timeout too low
        [InlineData(400000, 10000)] // Start timeout too high
        [InlineData(30000, 500)]    // Stop timeout too low
        [InlineData(30000, 70000)]  // Stop timeout too high
        public void Invalid_Timeout_Values_Should_Be_Detected(int startTimeout, int stopTimeout)
        {
            // Arrange
            var options = new AbpMassTransitOptions
            {
                StartTimeoutMilliSeconds = startTimeout,
                StopTimeoutMilliSeconds = stopTimeout
            };

            // Act & Assert
            Assert.False(options.IsValid());
        }
    }
}