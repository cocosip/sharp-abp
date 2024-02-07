using Xunit;

namespace SharpAbp.Abp.Crypto.SM3
{
    public class Sm3EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm3EncryptionService _sm3EncryptionService;

        public Sm3EncryptionServiceTest()
        {
            _sm3EncryptionService = GetRequiredService<ISm3EncryptionService>();
        }

        [Fact]
        public void GetHash_Test()
        {
            var plainText = "hello sm";
            var h = _sm3EncryptionService.GetHash(plainText);
            Assert.NotEmpty(h);
        }
    }
}
