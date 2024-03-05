using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialManagerTest : AbpTransformSecuritySM2TestBase
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly ISm2EncryptionService _sm2EncryptionService;
        private readonly ISecurityEncryptionService _securityEncryptionService;
        public SecurityCredentialManagerTest()
        {
            _securityCredentialManager = GetRequiredService<ISecurityCredentialManager>();
            _sm2EncryptionService = GetRequiredService<ISm2EncryptionService>();
            _securityEncryptionService = GetRequiredService<ISecurityEncryptionService>();
        }

        [Fact]
        public async Task Generate_SM2_Async_Test()
        {
            var securityCredential = await _securityCredentialManager.GenerateAsync("Login");
            var cipherText = _sm2EncryptionService.Encrypt(securityCredential.PublicKey, "HelloWorld");
            var plainText = _sm2EncryptionService.Decrypt(securityCredential.PrivateKey, cipherText);
            Assert.Equal("HelloWorld", plainText);
        }

        [Fact]
        public async Task Generate_SM2_Decrypt_Test_Async()
        {
            var securityCredential = await _securityCredentialManager.GenerateAsync("Login");

            var result = await _securityEncryptionService.ValidateAsync(securityCredential.Identifier);
            Assert.Equal(SecurityCredentialResultType.Success, result.Result);

            var cipherText = await _securityEncryptionService.EncryptAsync("SharpAbp", securityCredential.Identifier);
            var plainText = await _securityEncryptionService.DecryptAsync(cipherText, securityCredential.Identifier);
            Assert.Equal("SharpAbp", plainText);
        }
    }
}
