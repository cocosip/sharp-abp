using SharpAbp.Abp.Crypto.RSA;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialManagerTest : AbpTransformSecurityTestBase
    {
        private readonly ISecurityCredentialManager _securityCredentialManager;
        private readonly IRSAEncryptionService _rsaEncryptionService;
        private readonly ISecurityEncryptionService _securityEncryptionService;
        public SecurityCredentialManagerTest()
        {
            _securityCredentialManager = GetRequiredService<ISecurityCredentialManager>();
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
            _securityEncryptionService = GetRequiredService<ISecurityEncryptionService>();
        }

        [Fact]
        public async Task GenerateAsync_Test()
        {
            var securityCredential = await _securityCredentialManager.GenerateAsync("Login");
            var cipherText = _rsaEncryptionService.Encrypt(securityCredential.PublicKey, "HelloWorld");
            var plainText = _rsaEncryptionService.Decrypt(securityCredential.PrivateKey, cipherText);
            Assert.Equal("HelloWorld", plainText);
        }

        [Fact]
        public async Task Generate_Decrypt_Test_Async()
        {
            var securityCredential = await _securityCredentialManager.GenerateAsync("Login");

            var result = await _securityEncryptionService.ValidateAsync(securityCredential.Identifier);
            Assert.Equal(SecurityCredentialResultType.Success, result.Result);

            var cipherText = await _securityEncryptionService.EncryptAsync("1q2w3E*", securityCredential.Identifier);
            var plainText = await _securityEncryptionService.DecryptAsync(cipherText, securityCredential.Identifier);
            Assert.Equal("1q2w3E*", plainText);
        }

    }
}
