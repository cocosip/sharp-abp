using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.Timing;
using Xunit;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class DatabaseSecurityCredentialManagerTest : TransformSecurityManagementApplicationTestBase
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly ISecurityCredentialInfoRepository _securityCredentialInfoRepository;
        private readonly IRSACredsAppService _rsaCredsAppService;

        public DatabaseSecurityCredentialManagerTest()
        {
            _securityCredentialManager = GetRequiredService<ISecurityCredentialManager>();
            _securityCredentialInfoRepository = GetRequiredService<ISecurityCredentialInfoRepository>();
            _rsaCredsAppService = GetRequiredService<IRSACredsAppService>();
        }

        [Fact]
        public async Task GenerateAsync_Should_Create_And_Persist_Rsa_Credential()
        {
            await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto
            {
                Count = 1,
                Size = 2048
            });

            var credential = await _securityCredentialManager.GenerateAsync("Login");

            Assert.NotNull(credential);
            Assert.Equal(AbpTransformSecurityNames.RSA, credential.KeyType);
            Assert.Equal("Login", credential.BizType);
            Assert.False(string.IsNullOrWhiteSpace(credential.Identifier));
            Assert.False(string.IsNullOrWhiteSpace(credential.PublicKey));
            Assert.False(string.IsNullOrWhiteSpace(credential.PrivateKey));
            Assert.False(string.IsNullOrWhiteSpace(credential.GetReferenceId()));

            var info = await WithUnitOfWorkAsync(() =>
            {
                return _securityCredentialInfoRepository.FindByIdentifierAsync(credential.Identifier!);
            });

            Assert.NotNull(info);
            Assert.Equal(credential.Identifier, info.Identifier);
            Assert.Equal(Guid.Parse(credential.GetReferenceId()!), info.CredsId);
            Assert.Equal(AbpTransformSecurityNames.RSA, info.KeyType);
            Assert.Equal("Login", info.BizType);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task Imported_Rsa_Formats_Work_For_Generated_And_Reloaded_Transform_Credentials(bool pkcs8, bool pem)
        {
            var rsa = GetRequiredService<IRSAEncryptionService>();
            var pair = rsa.GenerateRSAKeyPair(2048);
            var publicKey = pem ? pair.ExportPublicKeyToPem() : pair.ExportPublicKey();
            var privateKey = pem
                ? (pkcs8 ? pair.ExportPrivateKeyPkcs8ToPem() : pair.ExportPrivateKeyToPem())
                : pair.ExportPrivateKeyPkcs8();
            var saved = await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 2048, PublicKey = publicKey, PrivateKey = privateKey
            });

            var generated = await _securityCredentialManager.GenerateAsync("Login");

            const string plaintext = "imported-vault-key-through-transform-security";
            var generatedCiphertext = rsa.EncryptFromBase64(generated.PublicKey, plaintext);
            Assert.Equal(plaintext, rsa.DecryptFromBase64(generated.PrivateKey, generatedCiphertext));
            var restored = await GetRequiredService<ISecurityCredentialStore>().GetAsync(generated.Identifier);
            Assert.Equal(generated.PublicKey, restored.PublicKey);
            Assert.Equal(generated.PrivateKey, restored.PrivateKey);
            Assert.Equal(saved.Id.ToString("N"), restored.GetReferenceId());
            Assert.Equal(generated.GetRSAKeySize(), restored.GetRSAKeySize());
            Assert.Equal(generated.GetRSAPadding(), restored.GetRSAPadding());
            var encryption = GetRequiredService<ISecurityEncryptionService>();
            var persistedCiphertext = await encryption.EncryptAsync(plaintext, restored.Identifier);
            Assert.Equal(plaintext, await encryption.DecryptAsync(persistedCiphertext, restored.Identifier));
            var original = await _rsaCredsAppService.GetDecryptKey(saved.Id);
            Assert.Equal(publicKey, original.PublicKey);
            Assert.Equal(privateKey, original.PrivateKey);
        }

        [Fact]
        public async Task Vault_Deletion_Rejects_A_Key_Referenced_By_Transform_Security()
        {
            await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto { Count = 1, Size = 2048 });
            var credential = await _securityCredentialManager.GenerateAsync("Login");
            var keyId = Guid.Parse(credential.GetReferenceId());

            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.DeleteAsync(keyId));
            Assert.NotNull(await _rsaCredsAppService.GetAsync(keyId));

            var info = await WithUnitOfWorkAsync(() => _securityCredentialInfoRepository.FindByIdentifierAsync(credential.Identifier));
            await GetRequiredService<ISecurityCredentialInfoAppService>().DeleteAsync(info.Id);
            await _rsaCredsAppService.DeleteAsync(keyId);
            Assert.Empty(await _rsaCredsAppService.GetListAsync());
        }

        [Fact]
        public async Task GenerateAsync_Forwards_Cancellation_To_The_Store()
        {
            await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto { Count = 1, Size = 2048 });
            var store = new RecordingStore();
            var manager = new DatabaseSecurityCredentialManager(
                GetRequiredService<IOptions<AbpTransformSecurityOptions>>(),
                GetRequiredService<IOptions<AbpTransformSecurityRSAOptions>>(),
                GetRequiredService<IOptions<AbpTransformSecuritySM2Options>>(),
                GetRequiredService<IGuidGenerator>(),
                GetRequiredService<IClock>(),
                GetRequiredService<IKeyService>(),
                store,
                GetRequiredService<IRSACredsRepository>(),
                GetRequiredService<ISM2CredsRepository>());
            using var cancellation = new CancellationTokenSource();

            await manager.GenerateAsync("Login", cancellation.Token);

            Assert.Equal(cancellation.Token, store.WriteToken);
        }

        private sealed class RecordingStore : ISecurityCredentialStore
        {
            public CancellationToken WriteToken { get; private set; }

            public Task<SecurityCredential> GetAsync(string identifier, CancellationToken cancellationToken = default)
            {
                throw new NotSupportedException();
            }

            public Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default)
            {
                WriteToken = cancellationToken;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task GenerateAsync_Should_Throw_For_Unsupported_BizType()
        {
            var exception = await Assert.ThrowsAsync<AbpException>(() =>
            {
                return _securityCredentialManager.GenerateAsync("UnknownBizType");
            });

            Assert.Contains("Unsupported bizType", exception.Message);
        }
    }
}
