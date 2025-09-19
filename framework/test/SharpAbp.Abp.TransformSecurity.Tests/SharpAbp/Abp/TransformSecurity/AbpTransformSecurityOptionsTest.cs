using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for AbpTransformSecurityOptions
    /// </summary>
    public class AbpTransformSecurityOptionsTest : AbpTransformSecurityTestBase
    {
        private readonly IOptions<AbpTransformSecurityOptions> _options;

        public AbpTransformSecurityOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpTransformSecurityOptions>>();
        }

        [Fact]
        public void Constructor_Should_Initialize_With_Default_Values()
        {
            // Act
            var options = new AbpTransformSecurityOptions();

            // Assert
            Assert.False(options.IsEnabled);
            Assert.Equal(AbpTransformSecurityNames.RSA, options.EncryptionAlgo);
            Assert.Equal(TimeSpan.FromSeconds(600), options.Expires);
            Assert.NotNull(options.BizTypes);
            Assert.Empty(options.BizTypes);
        }

        [Fact]
        public void Options_Should_Be_Properly_Configured()
        {
            // Arrange & Act
            var options = _options.Value;

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.BizTypes);
            Assert.True(options.IsEnabled); // Set in test module
            Assert.Equal(AbpTransformSecurityNames.RSA, options.EncryptionAlgo); // Set in test module
        }

        [Fact]
        public void Enabled_Property_Should_Be_Settable()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();

            // Act
            options.IsEnabled = true;

            // Assert
            Assert.True(options.IsEnabled);
        }

        [Fact]
        public void EncryptionAlgo_Property_Should_Be_Settable()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var algorithm = "SM2";

            // Act
            options.EncryptionAlgo = algorithm;

            // Assert
            Assert.Equal(algorithm, options.EncryptionAlgo);
        }

        [Fact]
        public void Expires_Property_Should_Be_Settable()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var expires = TimeSpan.FromMinutes(30);

            // Act
            options.Expires = expires;

            // Assert
            Assert.Equal(expires, options.Expires);
        }

        [Fact]
        public void BizTypes_Should_Allow_Adding_Items()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var bizType1 = "Login";
            var bizType2 = "UpdatePassword";

            // Act
            options.BizTypes.Add(bizType1);
            options.BizTypes.Add(bizType2);

            // Assert
            Assert.Equal(2, options.BizTypes.Count);
            Assert.Contains(bizType1, options.BizTypes);
            Assert.Contains(bizType2, options.BizTypes);
        }

        [Fact]
        public void BizTypes_Should_Allow_Removing_Items()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var bizType = "Login";
            options.BizTypes.Add(bizType);

            // Act
            var removed = options.BizTypes.Remove(bizType);

            // Assert
            Assert.True(removed);
            Assert.Empty(options.BizTypes);
        }

        [Fact]
        public void BizTypes_Should_Allow_Clearing_All_Items()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            options.BizTypes.Add("Login");
            options.BizTypes.Add("UpdatePassword");

            // Act
            options.BizTypes.Clear();

            // Assert
            Assert.Empty(options.BizTypes);
        }

        [Fact]
        public void BizTypes_Should_Support_Duplicate_Check()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var bizType = "Login";

            // Act
            options.BizTypes.Add(bizType);
            options.BizTypes.Add(bizType); // Add duplicate

            // Assert
            Assert.Equal(2, options.BizTypes.Count); // List allows duplicates
            Assert.All(options.BizTypes, item => Assert.Equal(bizType, item));
        }

        [Theory]
        [InlineData("RSA")]
        [InlineData("SM2")]
        [InlineData("AES")]
        public void EncryptionAlgo_Should_Accept_Various_Algorithms(string algorithm)
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();

            // Act
            options.EncryptionAlgo = algorithm;

            // Assert
            Assert.Equal(algorithm, options.EncryptionAlgo);
        }

        [Theory]
        [InlineData(60)] // 1 minute
        [InlineData(600)] // 10 minutes
        [InlineData(3600)] // 1 hour
        public void Expires_Should_Accept_Various_Timespan_Values(int seconds)
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();
            var timespan = TimeSpan.FromSeconds(seconds);

            // Act
            options.Expires = timespan;

            // Assert
            Assert.Equal(timespan, options.Expires);
        }

        [Fact]
        public void Options_Should_Support_Fluent_Configuration()
        {
            // Arrange & Act
            var options = new AbpTransformSecurityOptions
            {
                IsEnabled = true,
                EncryptionAlgo = "SM2",
                Expires = TimeSpan.FromMinutes(15)
            };
            options.BizTypes.Add("Login");
            options.BizTypes.Add("Register");

            // Assert
            Assert.True(options.IsEnabled);
            Assert.Equal("SM2", options.EncryptionAlgo);
            Assert.Equal(TimeSpan.FromMinutes(15), options.Expires);
            Assert.Equal(2, options.BizTypes.Count);
            Assert.Contains("Login", options.BizTypes);
            Assert.Contains("Register", options.BizTypes);
        }

        [Fact]
        public void BizTypes_Should_Be_Case_Sensitive()
        {
            // Arrange
            var options = new AbpTransformSecurityOptions();

            // Act
            options.BizTypes.Add("Login");
            options.BizTypes.Add("login");

            // Assert
            Assert.Equal(2, options.BizTypes.Count);
            Assert.Contains("Login", options.BizTypes);
            Assert.Contains("login", options.BizTypes);
        }

        [Fact]
        public void Default_Expires_Should_Be_600_Seconds()
        {
            // Arrange & Act
            var options = new AbpTransformSecurityOptions();

            // Assert
            Assert.Equal(600, options.Expires.TotalSeconds);
        }

        [Fact]
        public void Default_EncryptionAlgo_Should_Be_RSA()
        {
            // Arrange & Act
            var options = new AbpTransformSecurityOptions();

            // Assert
            Assert.Equal(AbpTransformSecurityNames.RSA, options.EncryptionAlgo);
        }

        [Fact]
        public void Options_Should_Be_Injected_Correctly()
        {
            // Arrange & Act
            var options = _options.Value;

            // Assert
            Assert.NotNull(options);
            Assert.True(options.IsEnabled); // Set in test module
            Assert.Equal(AbpTransformSecurityNames.RSA, options.EncryptionAlgo); // Set in test module
            Assert.Equal(TimeSpan.FromMinutes(10), options.Expires); // Set in test module
            Assert.NotNull(options.BizTypes);
            Assert.Contains("Login", options.BizTypes);
            Assert.Contains("UpdatePassword", options.BizTypes);
            Assert.Contains("TestBizType", options.BizTypes);
        }
    }
}