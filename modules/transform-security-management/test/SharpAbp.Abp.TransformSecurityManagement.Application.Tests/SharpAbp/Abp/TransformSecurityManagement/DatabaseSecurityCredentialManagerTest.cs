using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading.Tasks;
using Volo.Abp;
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
