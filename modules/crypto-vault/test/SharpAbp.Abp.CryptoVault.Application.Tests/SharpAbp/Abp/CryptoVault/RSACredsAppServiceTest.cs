using SharpAbp.Abp.Crypto.RSA;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;

namespace SharpAbp.Abp.CryptoVault
{
    public class RSACredsAppServiceTest : AbpCryptoVaultApplicationTestBase
    {
        private readonly IRSACredsAppService _rsaCredsAppService;
        private readonly IRSAEncryptionService _rsaEncryptionService;
        public RSACredsAppServiceTest()
        {
            _rsaCredsAppService = new RSACredsController(GetRequiredService<IRSACredsAppService>());
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

        [Fact]
        public async Task Import_Preserves_Pkcs8_Private_Keys_Exported_By_The_Crypto_Module()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKey = keyPair.ExportPublicKey();
            var privateKey = keyPair.ExportPrivateKeyPkcs8();

            var saved = await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 2048, PublicKey = publicKey, PrivateKey = privateKey
            });
            var restored = await _rsaCredsAppService.GetDecryptKey(saved.Id);

            Assert.Equal(publicKey, restored.PublicKey);
            Assert.Equal(privateKey, restored.PrivateKey);
            var plaintext = Encoding.UTF8.GetBytes("imported-key-roundtrip");
            var ciphertext = _rsaEncryptionService.Encrypt(_rsaEncryptionService.ImportPublicKey(restored.PublicKey), plaintext, "PKCS1Padding");
            Assert.Equal(plaintext, _rsaEncryptionService.Decrypt(_rsaEncryptionService.ImportPrivateKeyPkcs8(restored.PrivateKey), ciphertext, "PKCS1Padding"));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Import_Preserves_Pem_Keys_Exported_By_The_Crypto_Module(bool pkcs8)
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKey = keyPair.ExportPublicKeyToPem();
            var privateKey = pkcs8 ? keyPair.ExportPrivateKeyPkcs8ToPem() : keyPair.ExportPrivateKeyToPem();

            var saved = await _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 2048, PublicKey = publicKey, PrivateKey = privateKey
            });
            var restored = await _rsaCredsAppService.GetDecryptKey(saved.Id);

            Assert.Equal(publicKey, restored.PublicKey);
            Assert.Equal(privateKey, restored.PrivateKey);
            var plaintext = Encoding.UTF8.GetBytes("pem-key-roundtrip");
            var ciphertext = _rsaEncryptionService.Encrypt(_rsaEncryptionService.ImportPublicKeyPem(restored.PublicKey), plaintext, "PKCS1Padding");
            var restoredPrivate = pkcs8 ? _rsaEncryptionService.ImportPrivateKeyPkcs8Pem(restored.PrivateKey) : _rsaEncryptionService.ImportPrivateKeyPem(restored.PrivateKey);
            Assert.Equal(plaintext, _rsaEncryptionService.Decrypt(restoredPrivate, ciphertext, "PKCS1Padding"));
        }

        [Fact]
        public async Task Import_Rejects_Trailing_Data_Inside_A_Pem_Key()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var publicDer = Convert.FromBase64String(keyPair.ExportPublicKey());
            var privateDer = Convert.FromBase64String(keyPair.ExportPrivateKey());
            try
            {
                Array.Resize(ref publicDer, publicDer.Length + 1);
                Array.Resize(ref privateDer, privateDer.Length + 1);
                await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
                {
                    Size = 1024, PublicKey = PemEncoding.WriteString("PUBLIC KEY", publicDer), PrivateKey = keyPair.ExportPrivateKeyToPem()
                }));
                await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
                {
                    Size = 1024, PublicKey = keyPair.ExportPublicKeyToPem(), PrivateKey = PemEncoding.WriteString("RSA PRIVATE KEY", privateDer)
                }));
                Assert.Empty(await _rsaCredsAppService.GetListAsync());
            }
            finally
            {
                CryptographicOperations.ZeroMemory(privateDer);
            }
        }

        [Fact]
        public async Task Import_Rejects_Pem_Keys_In_The_Wrong_Role()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var privateKey = keyPair.ExportPrivateKeyToPem();
            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 1024, PublicKey = privateKey, PrivateKey = privateKey
            }));
            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(new CreateRSACredsDto
            {
                Size = 1024, PublicKey = keyPair.ExportPublicKeyToPem(), PrivateKey = keyPair.ExportPublicKeyToPem()
            }));
            Assert.Empty(await _rsaCredsAppService.GetListAsync());
        }

        [Fact]
        public async Task Import_Rejects_Invalid_Format_Mismatched_Pair_And_Wrong_Size()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var anotherPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var input = new CreateRSACredsDto
            {
                Size = 1024,
                PublicKey = "invalid",
                PrivateKey = keyPair.ExportPrivateKey()
            };

            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(input));
            input.PublicKey = anotherPair.ExportPublicKey();
            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(input));
            input.PublicKey = keyPair.ExportPublicKey();
            input.Size = 2048;
            await Assert.ThrowsAsync<UserFriendlyException>(() => _rsaCredsAppService.CreateAsync(input));
            Assert.Empty(await _rsaCredsAppService.GetListAsync());
        }

        [Fact]
        public async Task PublicKey_Above_MaxLength_Is_Rejected()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);

            await Assert.ThrowsAsync<AbpValidationException>(() => _rsaCredsAppService.CreateAsync(new CreateRSACredsDto()
            {
                Size = 1024,
                PublicKey = new string('a', RSACredsConsts.MaxPublicKeyLength + 1),
                PrivateKey = keyPair.Private.ExportPrivateKey()
            }));
        }

    }
}
