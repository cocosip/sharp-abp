using SharpAbp.Abp.Crypto.SM2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.CryptoVault
{
    public class SM2CredsAppServiceTest : AbpCryptoVaultApplicationTestBase
    {
        private readonly ISM2CredsAppService _sm2CredsAppService;
        private readonly ISm2EncryptionService _sm2EncryptionService;
        public SM2CredsAppServiceTest()
        {
            _sm2CredsAppService = GetRequiredService<ISM2CredsAppService>();
            _sm2EncryptionService = GetRequiredService<ISm2EncryptionService>();
        }

        [Fact]
        public async Task SM2_Curd_Async()
        {
            await _sm2CredsAppService.GenerateAsync(new GenerateSM2CredsDto()
            {
                Count = 2,
                Curve = Sm2EncryptionNames.CurveWapip192v1
            });


            var sm2Creds = await _sm2CredsAppService.GetListAsync(curve: Sm2EncryptionNames.CurveWapip192v1);
            Assert.Equal(2, sm2Creds.Count);

            var keyPair = _sm2EncryptionService.GenerateKeyPair(Sm2EncryptionNames.CurveWapip192v1);
            var pub = keyPair.Public.ExportPublicKey();
            var priv = keyPair.Private.ExportPrivateKey();

            var rsaCreds1 = await _sm2CredsAppService.CreateAsync(new CreateSM2CredsDto()
            {
                PublicKey = pub,
                PrivateKey = priv,
                Curve = Sm2EncryptionNames.CurveWapip192v1
            });

            var v1 = await _sm2CredsAppService.GetAsync(rsaCreds1.Id);
            Assert.NotEmpty(v1.PublicKey);
            Assert.NotEmpty(v1.PrivateKey);

            var sm2CredsList1 = await _sm2CredsAppService.GetListAsync(curve: Sm2EncryptionNames.CurveWapip192v1);
            Assert.Equal(3, sm2CredsList1.Count);

            foreach (var item in sm2CredsList1)
            {
                await _sm2CredsAppService.DeleteAsync(item.Id);
            }

            var sm2CredsList2 = await _sm2CredsAppService.GetListAsync(curve: Sm2EncryptionNames.CurveWapip192v1);
            Assert.Empty(sm2CredsList2);
        }

        [Fact]
        public async Task RSA_Encrypt_Decrypt_Async()
        {
            var keyPair = _sm2EncryptionService.GenerateKeyPair();

            var createDto = new CreateSM2CredsDto()
            {
                Curve = Sm2EncryptionNames.CurveSm2p256v1,
                PublicKey = keyPair.Public.ExportPublicKey(),
                PrivateKey = keyPair.Private.ExportPrivateKey(),
            };

            var sm2Creds = await _sm2CredsAppService.CreateAsync(createDto);
            var decryptKey = await _sm2CredsAppService.GetDecryptKey(sm2Creds.Id);
            Assert.NotEqual(sm2Creds.PublicKey, createDto.PublicKey);
            Assert.NotEqual(sm2Creds.PrivateKey, createDto.PrivateKey);
            Assert.Equal(decryptKey.PublicKey, createDto.PublicKey);
            Assert.Equal(decryptKey.PrivateKey, createDto.PrivateKey);
        }

    }
}
