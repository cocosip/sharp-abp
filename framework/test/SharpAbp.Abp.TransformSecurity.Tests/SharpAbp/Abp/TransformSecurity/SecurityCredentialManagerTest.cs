using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using System;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredentialManager
    /// </summary>
    public class SecurityCredentialManagerTest : AbpTransformSecurityTestBase
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly IRSAEncryptionService _rsaEncryptionService;
        private readonly ISecurityEncryptionService _securityEncryptionService;
        private readonly IClock _clock;

        public SecurityCredentialManagerTest()
        {
            _securityCredentialManager = GetRequiredService<ISecurityCredentialManager>();
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
            _securityEncryptionService = GetRequiredService<ISecurityEncryptionService>();
            _clock = GetRequiredService<IClock>();
        }

        [Fact]
        public async Task GenerateAsync_Should_Create_Valid_RSA_Credential()
        {
            // Arrange
            var bizType = "Login";

            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert
            Assert.NotNull(credential);
            Assert.NotNull(credential.Identifier);
            Assert.Equal(bizType, credential.BizType);
            Assert.Equal(AbpTransformSecurityNames.RSA, credential.KeyType);
            Assert.NotNull(credential.PublicKey);
            Assert.NotNull(credential.PrivateKey);
            Assert.True(credential.Expires > _clock.Now);
            Assert.True(credential.CreationTime <= _clock.Now);
            Assert.False(credential.IsExpires(_clock.Now));
        }

        [Fact]
        public async Task GenerateAsync_Should_Create_Functional_RSA_KeyPair()
        {
            // Arrange
            var bizType = "Login";
            var testMessage = "HelloWorld";

            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert - Test encryption/decryption functionality
            var publicKeyParam = _rsaEncryptionService.ImportPublicKey(credential.PublicKey!);
            var privateKeyParam = _rsaEncryptionService.ImportPrivateKey(credential.PrivateKey!);
            var cipherText = _rsaEncryptionService.Encrypt(publicKeyParam, Encoding.UTF8.GetBytes(testMessage), "PKCS1Padding");
            var plainText = Encoding.UTF8.GetString(_rsaEncryptionService.Decrypt(privateKeyParam, cipherText, "PKCS1Padding"));
            Assert.Equal(testMessage, plainText);
        }

        [Fact]
        public async Task GenerateAsync_Should_Throw_Exception_For_Invalid_BizType()
        {
            // Arrange
            var invalidBizType = "InvalidBizType";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AbpException>(
                () => _securityCredentialManager.GenerateAsync(invalidBizType));
            
            Assert.Contains("The business type", exception.Message);
            Assert.Contains("is not", exception.Message);
        }

        [Theory]
        [InlineData("Login")]
        [InlineData("UpdatePassword")]
        [InlineData("TestBizType")]
        public async Task GenerateAsync_Should_Work_For_Valid_BizTypes(string bizType)
        {
            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert
            Assert.NotNull(credential);
            Assert.Equal(bizType, credential.BizType);
        }

        [Fact]
        public async Task GenerateAsync_Should_Create_Unique_Identifiers()
        {
            // Arrange
            var bizType = "Login";

            // Act
            var credential1 = await _securityCredentialManager.GenerateAsync(bizType);
            var credential2 = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert
            Assert.NotEqual(credential1.Identifier, credential2.Identifier);
        }

        [Fact]
        public async Task GenerateAsync_Should_Set_Correct_RSA_Properties()
        {
            // Arrange
            var bizType = "Login";

            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert
            Assert.True(credential.IsRSA());
            Assert.False(credential.IsSM2());
            Assert.Equal(2048, credential.GetRSAKeySize());
            Assert.NotNull(credential.GetRSAPadding());
        }

        [Fact]
        public async Task GenerateAsync_Should_Store_Credential_In_Store()
        {
            // Arrange
            var bizType = "Login";

            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert - Verify credential can be validated through encryption service
            var validationResult = await _securityEncryptionService.ValidateAsync(credential.Identifier!);
            Assert.Equal(SecurityCredentialResultType.Success, validationResult.Result);
        }

        [Fact]
        public async Task GenerateAsync_Should_Work_With_EncryptionService()
        {
            // Arrange
            var bizType = "Login";
            var testMessage = "1q2w3E*";

            // Act
            var credential = await _securityCredentialManager.GenerateAsync(bizType);

            // Assert
            var validationResult = await _securityEncryptionService.ValidateAsync(credential.Identifier!);
            Assert.Equal(SecurityCredentialResultType.Success, validationResult.Result);

            var cipherText = await _securityEncryptionService.EncryptAsync(testMessage, credential.Identifier!);
            var plainText = await _securityEncryptionService.DecryptAsync(cipherText, credential.Identifier!);
            Assert.Equal(testMessage, plainText);
        }
    }
}
