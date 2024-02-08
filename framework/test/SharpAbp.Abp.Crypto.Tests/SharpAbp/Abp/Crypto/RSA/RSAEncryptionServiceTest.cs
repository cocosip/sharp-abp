
using Org.BouncyCastle.Utilities.Encoders;
using System.Text;
using Xunit;

namespace SharpAbp.Abp.Crypto.RSA
{
    public class RSAEncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly IRSAEncryptionService _rsaEncryptionService;
        public RSAEncryptionServiceTest()
        {
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
        }

        [Fact]
        public void Generate_Test()
        {
            var keyPair1 = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var pub1 = keyPair1.ExportPublicKey();
            var priv1 = keyPair1.ExportPrivateKey();
            var priv1_pkcs8 = keyPair1.ExportPrivateKeyPkcs8();
            var pub1_pem = keyPair1.ExportPublicKeyToPem();
            var priv1_pem = keyPair1.ExportPrivateKeyToPem();
            var priv1_pkcs8_pem = keyPair1.ExportPrivateKeyPkcs8ToPem();

            Assert.DoesNotContain("\n", pub1);
            Assert.DoesNotContain("\n", priv1);
            Assert.DoesNotContain("\n", priv1_pkcs8);
            Assert.Contains("\n", pub1_pem);
            Assert.NotEqual(priv1, priv1_pkcs8);
            Assert.Contains("\n", priv1_pem);
            Assert.Contains("\n", priv1_pkcs8_pem);


            _rsaEncryptionService.ImportPublicKey(pub1);
            _rsaEncryptionService.ImportPrivateKey(priv1);
        }

        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var pub = keyPair.ExportPublicKeyToPem();
            var priv = keyPair.ExportPrivateKeyToPem();
            var priv_pkcs8 = keyPair.ExportPrivateKeyPkcs8ToPem();

            var pub1 = keyPair.ExportPublicKey();
            var priv1 = keyPair.ExportPrivateKey();
            var priv1_pkcs8 = keyPair.ExportPrivateKeyPkcs8();

            var plainText = "这是中国!";

            var encrypted = _rsaEncryptionService.EncryptFromPem(pub, plainText, Encoding.UTF8);
            var decrypted = _rsaEncryptionService.DecryptFromPem(priv, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted);

            var decrypted2 = _rsaEncryptionService.DecryptFromPemPkcs8(priv_pkcs8, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted2);
        }

        [Fact]
        public void Encrypt_Decrypt_1024_Test()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var pub = keyPair.ExportPublicKeyToPem();
            var priv = keyPair.ExportPrivateKeyToPem();
            var priv_pkcs8 = keyPair.ExportPrivateKeyPkcs8ToPem();

            var pub1 = keyPair.ExportPublicKey();
            var priv1 = keyPair.ExportPrivateKey();
            var priv1_pkcs8 = keyPair.ExportPrivateKeyPkcs8();

            var plainText = "Hello World";

            var encrypted = _rsaEncryptionService.EncryptFromPem(pub, plainText, Encoding.UTF8);
            var decrypted = _rsaEncryptionService.DecryptFromPem(priv, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted);

            var decrypted2 = _rsaEncryptionService.DecryptFromPemPkcs8(priv_pkcs8, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted2);

            var encrypted11 = _rsaEncryptionService.Encrypt(pub1, plainText, Encoding.UTF8);
            var decrypted11 = _rsaEncryptionService.Decrypt(priv1, encrypted11, Encoding.UTF8);
            // Assert.Equal(plainText, decrypted11);
            //var decrypted12 = _rsaEncryptionService.DecryptFromPkcs8(priv1_pkcs8, encrypted11, Encoding.UTF8);

        }


        [Fact]
        public void Sign_Verify_Test()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var pub = keyPair.ExportPublicKeyToPem();
            var priv = keyPair.ExportPrivateKeyToPem();
            var priv_pkcs8 = keyPair.ExportPrivateKeyPkcs8ToPem();
        }

        [Fact]
        public void Test1()
        {
            var privateKeyPem = @"-----BEGIN PRIVATE KEY-----
MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBANRK3ZsgQHcfhCJRxa7wks4ekOedHJOaDQbf1ROZQ/EvsbBSdgvfZDTY/i9Tv4owkb2+CnRUMi2L+YC/DQGMq9L/4N18wOLv9nQTOzwV9bDyacD9Dd8sSBRkiRfp/9578+nNVYGaPLzgiRd3TVRc4I1KUzeZOaw2358QxuhvmSgFAgMBAAECgYBgwdaLE4IOSTECK178Qu3GQCwvRG5336i8T1xgWGSdEu3bvVnMQ376j0Qa8gLEyypB3tII/y73j2qigXE+GY1eAwhagxgiqpqvBTf+MlfCytLfvH/DmsZATVjcaovzSXexYzwwXNfxAxuEhQdy1hdatBD9omCxcSyamnfSds6fAQJBAPjVwcP773da0z4EquYQ2Bl91pxYHV8YhnKdp4f6IaqfrLPOnaZ7QQaKzcR1YLSr84bC6bv3ol/43uyfgwVwx2UCQQDaZ73qAn01nAr4bdoIgSDivwTDV+AM9+xCBsfkep+C/I4vgVcYn5SK6ueYpwheBJCdmV/Q9xDD+jUaEkVAvGQhAkEA3LJJzxdOBqAJp4HgSXk7ETDo/XWxZzyLUnC9u/5/iaNhO4DPlm7O94x4f/xTLysrLKUHRW2XGGPU5C19uX+TeQJANiaMns2ZL8aNrcTGz178wVttGeXaxjxeFozJ2OtSS07FDiA6cP93++18GIwpde4Z0QlrCUuIm56Ytesbwo4zIQJBAMYi4Zf+0EcS3BPk+o/tCkyBvWBXfycUryEWOge5tL10IJoGFkNzItURsGcR3GqHlESMhvnVFhXq9RFBfsX+aNI=
-----END PRIVATE KEY-----";



            var a = "YXJnZN4RiH7OjZjt3GnYBAOAdsqyKhToLIbm3JnzIwxlx1lhtUoA7hmTsiG839FAyy5B+mCSHgSZ+YZCs6QXxJFlZOXzDDhYZCbrUdFVoujJZnsg4SNC++vhdHPfxbA0U1ozTZGjIBcGOV0vqVQJci+Ds9mN352wui02toO0OcQ=";

            var v = _rsaEncryptionService.DecryptFromPem(privateKeyPem, a, Encoding.UTF8);

            Assert.NotEqual("", v);

        }


    }
}
