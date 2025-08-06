using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityEncryptionService
    /// </summary>
    public class SecurityEncryptionServiceTest : AbpTransformSecurityTestBase
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly ISecurityEncryptionService _securityEncryptionService;
        private readonly ISecurityCredentialStore _securityCredentialStore;
        private readonly IClock _clock;

        public SecurityEncryptionServiceTest()
        {
            _securityCredentialManager = GetRequiredService<ISecurityCredentialManager>();
            _securityEncryptionService = GetRequiredService<ISecurityEncryptionService>();
            _securityCredentialStore = GetRequiredService<ISecurityCredentialStore>();
            _clock = GetRequiredService<IClock>();
        }

        [Fact]
        public async Task ValidateAsync_Should_Return_Success_For_Valid_Credential()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");

            // Act
            var result = await _securityEncryptionService.ValidateAsync(credential.Identifier!);

            // Assert
            Assert.Equal(SecurityCredentialResultType.Success, result.Result);
        }

        [Fact]
        public async Task ValidateAsync_Should_Return_NotFound_For_Invalid_Identifier()
        {
            // Arrange
            var invalidIdentifier = Guid.NewGuid().ToString("N");

            // Act
            var result = await _securityEncryptionService.ValidateAsync(invalidIdentifier);

            // Assert
            Assert.Equal(SecurityCredentialResultType.NotFound, result.Result);
        }

        [Fact]
        public async Task ValidateAsync_Should_Return_Expired_For_Expired_Credential()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            
            // Manually set expiration to past
            credential.Expires = _clock.Now.AddMinutes(-1);
            await _securityCredentialStore.SetAsync(credential);

            // Act
            var result = await _securityEncryptionService.ValidateAsync(credential.Identifier!);

            // Assert
            Assert.Equal(SecurityCredentialResultType.Expired, result.Result);
        }

        [Fact]
        public async Task EncryptAsync_Should_Encrypt_PlainText_Successfully()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            var plainText = "TestMessage123";

            // Act
            var cipherText = await _securityEncryptionService.EncryptAsync(plainText, credential.Identifier!);

            // Assert
            Assert.NotNull(cipherText);
            Assert.NotEmpty(cipherText);
            Assert.NotEqual(plainText, cipherText);
        }

        [Fact]
        public async Task DecryptAsync_Should_Decrypt_CipherText_Successfully()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            var plainText = "TestMessage123";
            var cipherText = await _securityEncryptionService.EncryptAsync(plainText, credential.Identifier!);

            // Act
            var decryptedText = await _securityEncryptionService.DecryptAsync(cipherText, credential.Identifier!);

            // Assert
            Assert.Equal(plainText, decryptedText);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hello World")]
        [InlineData("1234567890")]
        [InlineData("Special@#$%^&*()Characters")]
        [InlineData("中文测试")]
        public async Task EncryptDecrypt_Should_Work_For_Various_Inputs(string testInput)
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");

            // Act
            var cipherText = await _securityEncryptionService.EncryptAsync(testInput, credential.Identifier!);
            var decryptedText = await _securityEncryptionService.DecryptAsync(cipherText, credential.Identifier!);

            // Assert
            Assert.Equal(testInput, decryptedText);
        }

        [Fact]
        public async Task EncryptAsync_Should_Throw_Exception_For_Invalid_Identifier()
        {
            // Arrange
            var invalidIdentifier = Guid.NewGuid().ToString("N");
            var plainText = "TestMessage";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AbpException>(
                () => _securityEncryptionService.EncryptAsync(plainText, invalidIdentifier));
            
            Assert.Contains("Could not find security key by id", exception.Message);
        }

        [Fact]
        public async Task DecryptAsync_Should_Throw_Exception_For_Invalid_Identifier()
        {
            // Arrange
            var invalidIdentifier = Guid.NewGuid().ToString("N");
            var cipherText = "InvalidCipherText";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AbpException>(
                () => _securityEncryptionService.DecryptAsync(cipherText, invalidIdentifier));
            
            Assert.Contains("Could not find security key by id", exception.Message);
        }

        [Fact]
        public async Task EncryptAsync_Should_Produce_Different_Results_For_Same_Input()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            var plainText = "TestMessage";

            // Act
            var cipherText1 = await _securityEncryptionService.EncryptAsync(plainText, credential.Identifier!);
            var cipherText2 = await _securityEncryptionService.EncryptAsync(plainText, credential.Identifier!);

            // Assert - RSA encryption with padding should produce different results for same input
            Assert.NotEqual(cipherText1, cipherText2);
            
            // But both should decrypt to the same plaintext
            var decrypted1 = await _securityEncryptionService.DecryptAsync(cipherText1, credential.Identifier!);
            var decrypted2 = await _securityEncryptionService.DecryptAsync(cipherText2, credential.Identifier!);
            Assert.Equal(plainText, decrypted1);
            Assert.Equal(plainText, decrypted2);
        }

        [Fact]
        public async Task EncryptDecrypt_Should_Work_With_Long_Text()
        {
            // Arrange
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            // Create a text that's close to RSA key size limit but still valid
            var longText = new string('A', 100); // 100 characters should be safe for 2048-bit RSA

            // Act
            var cipherText = await _securityEncryptionService.EncryptAsync(longText, credential.Identifier!);
            var decryptedText = await _securityEncryptionService.DecryptAsync(cipherText, credential.Identifier!);

            // Assert
            Assert.Equal(longText, decryptedText);
        }
    }
}