using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System;
using Volo.Abp.Caching;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for AbpTransformSecurityModule
    /// </summary>
    public class AbpTransformSecurityModuleTest : AbpTransformSecurityTestBase
    {
        [Fact]
        public void Module_Should_Register_Required_Services()
        {
            // Act & Assert
            Assert.NotNull(GetService<ISecurityCredentialManager>());
            Assert.NotNull(GetService<ISecurityEncryptionService>());
            Assert.NotNull(GetService<ISecurityCredentialStore>());
            Assert.NotNull(GetService<IRSAEncryptionService>());
            Assert.NotNull(GetService<ISm2EncryptionService>());
        }

        [Fact]
        public void Module_Should_Configure_TransformSecurityOptions()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpTransformSecurityOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.True(options.Enabled); // Set in test module
            Assert.Equal("RSA", options.EncryptionAlgo);
            Assert.Equal(TimeSpan.FromMinutes(10), options.Expires);
            Assert.Contains("Login", options.BizTypes);
            Assert.Contains("UpdatePassword", options.BizTypes);
            Assert.Contains("TestBizType", options.BizTypes);
        }

        [Fact]
        public void Module_Should_Configure_RSAOptions()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpTransformSecurityRSAOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.Equal(RSAPaddingNames.PKCS1Padding, options.Padding);
            Assert.Equal(2048, options.KeySize);
        }

        [Fact]
        public void Module_Should_Configure_SM2Options()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpTransformSecuritySM2Options>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.Equal(Sm2EncryptionNames.CurveSm2p256v1, options.Curve);
            Assert.Equal(Org.BouncyCastle.Crypto.Engines.SM2Engine.Mode.C1C2C3, options.Mode);
        }

        [Fact]
        public void Module_Should_Configure_DistributedCacheOptions()
        {
            // Act
            var options = GetRequiredService<IOptions<AbpDistributedCacheOptions>>().Value;

            // Assert
            Assert.NotNull(options);
            Assert.NotEmpty(options.CacheConfigurators);
            
            // Test cache configurator for SecurityCredential
            var cacheName = CacheNameAttribute.GetCacheName(typeof(SecurityCredential));
            var cacheOptions = options.CacheConfigurators[0](cacheName);
            
            Assert.NotNull(cacheOptions);
            Assert.Equal(TimeSpan.FromSeconds(900), cacheOptions.SlidingExpiration);
        }

        [Fact]
        public void Services_Should_Be_Registered_As_Transient()
        {
            // Act
            var manager1 = GetService<ISecurityCredentialManager>();
            var manager2 = GetService<ISecurityCredentialManager>();
            
            var service1 = GetService<ISecurityEncryptionService>();
            var service2 = GetService<ISecurityEncryptionService>();

            // Assert - Transient services should be different instances
            Assert.NotSame(manager1, manager2);
            Assert.NotSame(service1, service2);
        }

        [Fact]
        public void Module_Should_Have_Correct_Dependencies()
        {
            // This test verifies that the module can be loaded with its dependencies
            // If dependencies are missing, the test setup would fail
            
            // Act & Assert - If we reach here, dependencies are correctly configured
            Assert.True(true);
        }

        [Fact]
        public void CacheConfigurator_Should_Return_Null_For_Unknown_CacheNames()
        {
            // Arrange
            var options = GetRequiredService<IOptions<AbpDistributedCacheOptions>>().Value;
            var unknownCacheName = "UnknownCache";

            // Act
            var cacheOptions = options.CacheConfigurators[0](unknownCacheName);

            // Assert
            Assert.Null(cacheOptions);
        }

        [Fact]
        public void All_Required_Interfaces_Should_Have_Implementations()
        {
            // Act & Assert
            Assert.IsAssignableFrom<SecurityCredentialManager>(
                GetRequiredService<ISecurityCredentialManager>());
            
            Assert.IsAssignableFrom<SecurityEncryptionService>(
                GetRequiredService<ISecurityEncryptionService>());
            
            Assert.IsAssignableFrom<SecurityCredentialStore>(
                GetRequiredService<ISecurityCredentialStore>());
        }

        [Fact]
        public void Options_Should_Be_Properly_Injected_Into_Services()
        {
            // Arrange
            var manager = GetRequiredService<ISecurityCredentialManager>() as SecurityCredentialManager;
            var service = GetRequiredService<ISecurityEncryptionService>() as SecurityEncryptionService;

            // Act & Assert
            Assert.NotNull(manager);
            Assert.NotNull(service);
            
            // The services should be able to function properly with injected options
            // This is tested indirectly through other functional tests
        }
    }
}