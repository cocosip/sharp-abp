using SharpAbp.Abp.Crypto.RSA;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACredsAppServiceTest : AbpCryptoVaultApplicationTestBase
    {
        private readonly IRSACredsAppService _rsaCredsAppService;
        private readonly IRSAEncryptionService _rsaEncryptionService;
        public RSACredsAppServiceTest()
        {
            _rsaCredsAppService = GetRequiredService<IRSACredsAppService>();
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
        }

        [Fact]
        public async Task RSA_Curd_Async()
        {
            await _rsaCredsAppService.GenerateAsync(new GenerateRSACredsDto()
            {
                Count = 2,
                Size = 1024
            });


            var rsaCreds = await _rsaCredsAppService.GetListAsync(size: 1024);
            Assert.Equal(2, rsaCreds.Count);

            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var pub = keyPair.Public.ExportPublicKey();
            var priv = keyPair.Private.ExportPrivateKey();

            var rsaCreds1 = await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto()
            {
                PublicKey = pub,
                PrivateKey = priv,
                Size = 1024
            });

            var v1 = await _rsaCredsAppService.GetAsync(rsaCreds1.Id);
            Assert.NotEmpty(v1.PublicKey);
            Assert.NotEmpty(v1.PrivateKey);

            var rsaCredsList1 = await _rsaCredsAppService.GetListAsync(size: 1024);
            Assert.Equal(3, rsaCredsList1.Count);

            foreach (var item in rsaCredsList1)
            {
                await _rsaCredsAppService.DeleteAsync(item.Id);
            }

            var rsaCredsList2 = await _rsaCredsAppService.GetListAsync(size: 1024);
            Assert.Empty(rsaCredsList2);
        }

        [Fact]
        public async Task RSA_Encrypt_Decrypt_Async()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);

            var createDto = new CreateRSACredsDto()
            {
                Size = 2048,
                PublicKey = keyPair.Public.ExportPublicKey(),
                PrivateKey = keyPair.Private.ExportPrivateKey(),
            };

            var rsaCreds = await _rsaCredsAppService.CreateAsync(createDto);
            var decryptKey = await _rsaCredsAppService.GetDecryptKey(rsaCreds.Id);
            Assert.NotEqual(rsaCreds.PublicKey, createDto.PublicKey);
            Assert.NotEqual(rsaCreds.PrivateKey, createDto.PrivateKey);
            Assert.Equal(decryptKey.PublicKey, createDto.PublicKey);
            Assert.Equal(decryptKey.PrivateKey, createDto.PrivateKey);
        }

    }
}
