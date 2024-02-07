using Xunit;

namespace SharpAbp.Abp.Crypto.SM4
{
    public class Sm4EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm4EncryptionService _sm4EncryptionService;
        public Sm4EncryptionServiceTest()
        {
            _sm4EncryptionService = GetRequiredService<ISm4EncryptionService>();
        }

        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            var key = "abcdefghijklmnop";
            var iv = "1234567890abcdef";

            var plainText = "0123456789HelloWorld1234567890ABCDEF";

            var cipherText = _sm4EncryptionService.Encrypt(plainText, key, iv, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            var source = _sm4EncryptionService.Decrypt(cipherText, key, iv, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            Assert.Equal(plainText, source);
        }
    }
}
