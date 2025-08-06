using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredentialStore
    /// </summary>
    public class SecurityCredentialStoreTest : AbpTransformSecurityTestBase
    {
        private readonly ISecurityCredentialStore _store;
        private readonly ISecurityCredentialManager _credentialManager;

        public SecurityCredentialStoreTest()
        {
            _store = GetRequiredService<ISecurityCredentialStore>();
            _credentialManager = GetRequiredService<ISecurityCredentialManager>();
        }

        [Fact]
        public async Task GetAsync_Should_Return_Credential_From_Store()
        {
            // Arrange
            var credential = await _credentialManager.GenerateAsync("Login");
            var identifier = credential.Identifier!;

            // Act
            var result = await _store.GetAsync(identifier);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(credential.Identifier, result.Identifier);
            Assert.Equal(credential.KeyType, result.KeyType);
            Assert.Equal(credential.BizType, result.BizType);
            Assert.Equal(credential.PublicKey, result.PublicKey);
            Assert.Equal(credential.PrivateKey, result.PrivateKey);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            var identifier = Guid.NewGuid().ToString("N");

            // Act
            var result = await _store.GetAsync(identifier);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_Should_Pass_CancellationToken()
        {
            // Arrange
            var credential = await _credentialManager.GenerateAsync("Login");
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _store.GetAsync(credential.Identifier!, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(credential.Identifier, result.Identifier);
        }

        [Fact]
        public async Task SetAsync_Should_Store_Credential()
        {
            // Arrange
            var credential = await _credentialManager.GenerateAsync("Login");

            // Act
            await _store.SetAsync(credential);

            // Assert - Verify by retrieving
            var retrieved = await _store.GetAsync(credential.Identifier!);
            Assert.NotNull(retrieved);
            Assert.Equal(credential.Identifier, retrieved.Identifier);
        }

        [Fact]
        public async Task SetAsync_Should_Pass_CancellationToken()
        {
            // Arrange
            var credential = await _credentialManager.GenerateAsync("Login");
            var cancellationToken = new CancellationToken();

            // Act & Assert - Should not throw
            await _store.SetAsync(credential, cancellationToken);
            
            var retrieved = await _store.GetAsync(credential.Identifier!);
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task Store_Should_Support_Multiple_Credentials()
        {
            // Arrange
            var credential1 = await _credentialManager.GenerateAsync("Login");
            var credential2 = await _credentialManager.GenerateAsync("UpdatePassword");

            // Act
            await _store.SetAsync(credential1);
            await _store.SetAsync(credential2);

            // Assert
            var retrieved1 = await _store.GetAsync(credential1.Identifier!);
            var retrieved2 = await _store.GetAsync(credential2.Identifier!);
            
            Assert.NotNull(retrieved1);
            Assert.NotNull(retrieved2);
            Assert.Equal(credential1.Identifier, retrieved1.Identifier);
            Assert.Equal(credential2.Identifier, retrieved2.Identifier);
            Assert.NotEqual(retrieved1.Identifier, retrieved2.Identifier);
        }

        [Fact]
        public async Task GetAsync_And_SetAsync_Should_Work_Together()
        {
            // Arrange
            var credential = await _credentialManager.GenerateAsync("Login");

            // Act
            await _store.SetAsync(credential);
            var retrievedCredential = await _store.GetAsync(credential.Identifier!);

            // Assert
            Assert.NotNull(retrievedCredential);
            Assert.Equal(credential.Identifier, retrievedCredential.Identifier);
            Assert.Equal(credential.KeyType, retrievedCredential.KeyType);
            Assert.Equal(credential.BizType, retrievedCredential.BizType);
            Assert.Equal(credential.PublicKey, retrievedCredential.PublicKey);
            Assert.Equal(credential.PrivateKey, retrievedCredential.PrivateKey);
        }

        [Fact]
        public async Task SetAsync_Should_Overwrite_Existing_Credential()
        {
            // Arrange
            var credential1 = await _credentialManager.GenerateAsync("Login");
            var credential2 = await _credentialManager.GenerateAsync("Login");
            
            // Use same identifier for both
            credential2.Identifier = credential1.Identifier;

            // Act
            await _store.SetAsync(credential1);
            await _store.SetAsync(credential2); // Overwrite

            // Assert
            var retrieved = await _store.GetAsync(credential1.Identifier!);
            Assert.NotNull(retrieved);
            Assert.Equal(credential2.PublicKey, retrieved.PublicKey);
            Assert.Equal(credential2.PrivateKey, retrieved.PrivateKey);
        }

        [Fact]
        public void Store_Should_Be_Properly_Injected()
        {
            // Assert
            Assert.NotNull(_store);
            Assert.IsAssignableFrom<SecurityCredentialStore>(_store);
        }
    }
}