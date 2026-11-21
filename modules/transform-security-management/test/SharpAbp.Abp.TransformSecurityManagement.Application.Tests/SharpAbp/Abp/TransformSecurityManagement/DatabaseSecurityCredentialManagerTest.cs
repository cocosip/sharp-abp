using Microsoft.Extensions.Options;
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
