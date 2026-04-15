using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.CryptoVault;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class DatabaseSecurityCredentialStoreTest : TransformSecurityManagementApplicationTestBase
    {
        private readonly ISecurityCredentialStore _securityCredentialStore;
        private readonly ISecurityCredentialInfoRepository _securityCredentialInfoRepository;
        private readonly IRSACredsAppService _rsaCredsAppService;
        private readonly IRSAEncryptionService _rsaEncryptionService;

        public DatabaseSecurityCredentialStoreTest()
        {
            _securityCredentialStore = GetRequiredService<ISecurityCredentialStore>();
            _securityCredentialInfoRepository = GetRequiredService<ISecurityCredentialInfoRepository>();
            _rsaCredsAppService = GetRequiredService<IRSACredsAppService>();
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
        }

        [Fact]
        public async Task SetAsync_And_GetAsync_Should_RoundTrip_Rsa_Credential()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKey = keyPair.Public.ExportPublicKey();
            var privateKey = keyPair.Private.ExportPrivateKey();

            var rsaCreds = await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 2048,
                PublicKey = publicKey,
                PrivateKey = privateKey
            });

            var credential = new SecurityCredential
            {
                Identifier = "login-credential",
                KeyType = AbpTransformSecurityNames.RSA,
                BizType = "Login",
                CreationTime = new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                Expires = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc)
            }.SetReferenceId(rsaCreds.Id.ToString("N"));

            await _securityCredentialStore.SetAsync(credential);

            var info = await WithUnitOfWorkAsync(() =>
            {
                return _securityCredentialInfoRepository.FindByIdentifierAsync("login-credential");
            });

            Assert.NotNull(info);
            Assert.Equal(rsaCreds.Id, info.CredsId);

            var result = await _securityCredentialStore.GetAsync("login-credential");

            Assert.Equal("login-credential", result.Identifier);
            Assert.Equal(AbpTransformSecurityNames.RSA, result.KeyType);
            Assert.Equal("Login", result.BizType);
            Assert.Equal(info.CreationTime, result.CreationTime);
            Assert.Equal(credential.Expires, result.Expires);
            Assert.Equal(publicKey, result.PublicKey);
            Assert.Equal(privateKey, result.PrivateKey);
            Assert.Equal(rsaCreds.Id.ToString("N"), result.GetReferenceId());
        }
    }
}
