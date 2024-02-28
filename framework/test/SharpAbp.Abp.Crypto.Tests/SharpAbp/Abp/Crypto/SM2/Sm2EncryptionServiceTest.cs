using Xunit;

namespace SharpAbp.Abp.Crypto.SM2
{
    public class Sm2EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm2EncryptionService _sm2EncryptionService;

        public Sm2EncryptionServiceTest()
        {
            _sm2EncryptionService = GetRequiredService<ISm2EncryptionService>();
        }

        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var aPublic = keyPair.ExportPublicKey();
            var aPrivate = keyPair.ExportPrivateKey();

            var plainText = "hello word, it's sm";
            var cipherText = _sm2EncryptionService.Encrypt(aPublic, plainText);
            var source = _sm2EncryptionService.Decrypt(aPrivate, cipherText);
            Assert.Equal(plainText, source);
        }

        [Fact]
        public void Decrypt_From_Third_Test()
        {
            var aPublic =
                "04348F0E358DE83E7804D2898D6B4A475BF99FCAB3D5B24D7A8701F60CBD54662D3B02106FC8E924343C089FDDE14CC7CBC3C252AF3BFA28EDB3DE7397CF171F45";
            var aPrivate = "008E47213A4E78F800949B4B3F975D5C0F54684BAD16DDA9430A1817C146F327A4";

            var data = "{\"hospitalCode\":\"123717224954206128\",\"recordNum\":\"220223033\"}";
            var r = _sm2EncryptionService.Encrypt(aPublic, data, "utf-8").ToUpper();
            var t1 =
                "04B05D05B58E29223CAF42C6BB615929B678EEDB4B7E18DB7E74BD017CA8EE7EAFD54D06E7E6D2A8B98B106325596149374C9DF4AE5E0083D34B89912FE53C0BC60515FAAAC537E0FEE7A367F74B77936B23CE20BC51881308E1925AB821D40B0478629F23127C";
            var s1 = _sm2EncryptionService.Decrypt(aPrivate, t1);
            Assert.Equal("张三", s1);

            var s2 = _sm2EncryptionService.Decrypt(aPrivate, r, "utf-8");
            Assert.Equal(data, s2);
        }

        [Fact]
        public void Sign_Verify_Test()
        {
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var aPublic = keyPair.ExportPublicKey();
            var aPrivate = keyPair.ExportPrivateKey();

            var plainText = "hello word";
            var signed = _sm2EncryptionService.Sign(aPrivate, plainText);
            var r = _sm2EncryptionService.VerifySign(aPublic, plainText, signed);
            // Assert.True(r);
        }
    }
}
