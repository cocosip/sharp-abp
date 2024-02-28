using Org.BouncyCastle.Tls;
using System.IO;
using System.Net;
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
            _rsaEncryptionService.ImportPublicKeyPem(pub1_pem);
            _rsaEncryptionService.ImportPrivateKeyPem(priv1_pem);
            _rsaEncryptionService.ImportPrivateKeyPkcs8(priv1_pkcs8);
            _rsaEncryptionService.ImportPrivateKeyPkcs8Pem(priv1_pkcs8_pem);
        }

        [Fact]
        public void Key_Import_Export_Test()
        {
            var pkcs8Key = @"MIICeQIBADANBgkqhkiG9w0BAQEFAASCAmMwggJfAgEAAoGBAMz0Czg6QUtTISa2pUkloeQB/TEpHdqrfyroWpKLW9B/LWFSOGH9nyTk1pPZaeadyEZQ6gay/C0pUAetLraq9bMA/Luxq68b87uG7WX7dKytEO2/87qGpGMRs97H+GlkzWil2QO2KK4cHnAcVicPsmi5aZ72U0BWJFyPhtd+qdmrAgMBAAECgYEAvW67iAbgHt0BASVD9C3iSjpEaVHVlC165o/IVzaTcEx8Bz3Ve0zN8W3JnvIO3ebsG4HiLLr2Nk++9rltOc0eNeGMv7F1e/OFot1wN0ON6s1g4bYh1z5Uz8FcYiMWcqHHICrx+oSFeK9x+I2Zge7enQXcsVnqEhm77ZE5YczSryECQQD9nB58e5efYchF+cYbmURioX18cUMuhQbB9Aq2N55cd689Lg35KZqT8JQTp/8tQSdCJG8d2nU8VKspUKTEAuaDAkEAzuKIIoc9PVJvy90LhIPA9c1S8BPCI7EMCaTZqJ5o3VaR2dqvUZDGX7kL3kYkQ+n7mq3KIECvkEFzA+FOP96XuQJBAJQTKHW0T/YeSKoayUHp/lS8R6F2HCy4PRbXn71+wfbpZqcJEd2OHhQM3tiPOV258esbjMlYeSUNppZL4LgVnXMCQQC7Lvs9Ql+GPDAqo7ToEM1lmICR906QPIBHuX+1sJ3wpYMROWumwPa7ZRH36j6ls+6R5OwcgmpWeuE1gYTrBNsBAkEAn2pEtAljX1foQff6CLozYg/J6J9RmVFcJ6qz0LX3052qNFBQYw8CMHB7VkVNzsDIDC8LX5uP2pzTrdPLew+pPA==";

            var pkcs1Key = @"MIICXwIBAAKBgQDM9As4OkFLUyEmtqVJJaHkAf0xKR3aq38q6FqSi1vQfy1hUjhh/Z8k5NaT2WnmnchGUOoGsvwtKVAHrS62qvWzAPy7sauvG/O7hu1l+3SsrRDtv/O6hqRjEbPex/hpZM1opdkDtiiuHB5wHFYnD7JouWme9lNAViRcj4bXfqnZqwIDAQABAoGBAL1uu4gG4B7dAQElQ/Qt4ko6RGlR1ZQteuaPyFc2k3BMfAc91XtMzfFtyZ7yDt3m7BuB4iy69jZPvva5bTnNHjXhjL+xdXvzhaLdcDdDjerNYOG2Idc+VM/BXGIjFnKhxyAq8fqEhXivcfiNmYHu3p0F3LFZ6hIZu+2ROWHM0q8hAkEA/ZwefHuXn2HIRfnGG5lEYqF9fHFDLoUGwfQKtjeeXHevPS4N+Smak/CUE6f/LUEnQiRvHdp1PFSrKVCkxALmgwJBAM7iiCKHPT1Sb8vdC4SDwPXNUvATwiOxDAmk2aieaN1Wkdnar1GQxl+5C95GJEPp+5qtyiBAr5BBcwPhTj/el7kCQQCUEyh1tE/2HkiqGslB6f5UvEehdhwsuD0W15+9fsH26WanCRHdjh4UDN7YjzldufHrG4zJWHklDaaWS+C4FZ1zAkEAuy77PUJfhjwwKqO06BDNZZiAkfdOkDyAR7l/tbCd8KWDETlrpsD2u2UR9+o+pbPukeTsHIJqVnrhNYGE6wTbAQJBAJ9qRLQJY19X6EH3+gi6M2IPyeifUZlRXCeqs9C199OdqjRQUGMPAjBwe1ZFTc7AyAwvC1+bj9qc063Ty3sPqTw=";


            var privateKey1 = _rsaEncryptionService.ImportPrivateKeyPkcs8(pkcs8Key);
            var privateKey2 = _rsaEncryptionService.ImportPrivateKey(pkcs1Key);
        }



        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var pub = keyPair.ExportPublicKeyToPem();
            var priv = keyPair.ExportPrivateKeyToPem();
            var priv_pkcs8 = keyPair.ExportPrivateKeyPkcs8ToPem();

            var plainText = "这是中国!";

            var encrypted = _rsaEncryptionService.EncryptFromPem(pub, plainText, Encoding.UTF8);
            var decrypted = _rsaEncryptionService.DecryptFromPem(priv, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted);

            var decrypted2 = _rsaEncryptionService.DecryptFromPkcs8Pem(priv_pkcs8, encrypted, Encoding.UTF8);
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

            var decrypted2 = _rsaEncryptionService.DecryptFromPkcs8Pem(priv_pkcs8, encrypted, Encoding.UTF8);
            Assert.Equal(plainText, decrypted2);

            var encrypted11 = _rsaEncryptionService.Encrypt(pub1, plainText, Encoding.UTF8);
            var decrypted11 = _rsaEncryptionService.Decrypt(priv1, encrypted11, Encoding.UTF8);
            Assert.Equal(plainText, decrypted11);
            var decrypted12 = _rsaEncryptionService.DecryptFromPkcs8(priv1_pkcs8, encrypted11, Encoding.UTF8);
            Assert.Equal(plainText, decrypted12);
        }


        [Fact]
        public void Sign_Verify_Test()
        {
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(1024);
            var pub = keyPair.ExportPublicKey();
            var priv = keyPair.ExportPrivateKey();
            var priv_pkcs8 = keyPair.ExportPrivateKeyPkcs8();
            var pub_pem = keyPair.ExportPublicKeyToPem();
            var priv_pem = keyPair.ExportPrivateKeyToPem();
            var priv_pkcs8_pem = keyPair.ExportPrivateKeyPkcs8ToPem();

            var data = "This is ZHANGSAN";
            var signature1 = _rsaEncryptionService.Sign(priv, data);
            var r1 = _rsaEncryptionService.VerifySign(pub, data, signature1);
            Assert.True(r1);

            var signature2 = _rsaEncryptionService.SignFromPkcs8(priv_pkcs8, data);
            var r2 = _rsaEncryptionService.VerifySign(pub, data, signature2);
            Assert.True(r2);

            var signature3 = _rsaEncryptionService.SignFromPem(priv_pem, data);
            var r3 = _rsaEncryptionService.VerifySign(pub, data, signature3);
            Assert.True(r3);

            var signature4 = _rsaEncryptionService.SignFromPkcs8Pem(priv_pkcs8_pem, data);
            var r4 = _rsaEncryptionService.VerifySign(pub, data, signature4);
            Assert.True(r4);
        }

        [Fact]
        public void Encrypt_Decrypt_Test1()
        {

//            var pub = @"-----BEGIN PUBLIC KEY-----
//MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnTHIt0Yyam+QKjC6TOeR
//lpLUXH6HKX4NBNauEIluuafk7MVHL8rOCOdWGkF8DLgAGubyjYlXsav9RJcWi67j
//qyxOAkloFHyrQbP3DcahQvUi+PB+gNhIpdEibohPXyPEkbRGTp7Zb1F13Ya9bZlT
//dHsh+ge220pTtFnjYcC06cOZ1M7biDUMzCL/nhHbcXYYWIxPcDW3XMElQEBV9Ths
//nnigiuFG9N/aWYbI+Ogr/JxqJdrO0DvMuvigXJGH2WYfhREtNyr1PUPd/9Kj2kkL
//fOpihfVTjWeJ4cndkiFuLbyjZeRyUGVWp55Fn2ltFjMW89yAAfbIy4QYSM1KBmz7
//ZwIDAQAB
//-----END PUBLIC KEY-----";
            var priv = @"-----BEGIN RSA PRIVATE KEY-----
MIIEogIBAAKCAQEAnTHIt0Yyam+QKjC6TOeRlpLUXH6HKX4NBNauEIluuafk7MVH
L8rOCOdWGkF8DLgAGubyjYlXsav9RJcWi67jqyxOAkloFHyrQbP3DcahQvUi+PB+
gNhIpdEibohPXyPEkbRGTp7Zb1F13Ya9bZlTdHsh+ge220pTtFnjYcC06cOZ1M7b
iDUMzCL/nhHbcXYYWIxPcDW3XMElQEBV9ThsnnigiuFG9N/aWYbI+Ogr/JxqJdrO
0DvMuvigXJGH2WYfhREtNyr1PUPd/9Kj2kkLfOpihfVTjWeJ4cndkiFuLbyjZeRy
UGVWp55Fn2ltFjMW89yAAfbIy4QYSM1KBmz7ZwIDAQABAoIBADN+8yejLfHzUVGY
+/ckp2uh90LDyoibvC0ZHRXax/S3HUY5jIwKDrwY+PqJ+Fb8UkB95vjaBOn2E3bM
XjztUrUpQvb50Ehh3QKdr7IKH5sdTlMqCe8wq3/yxqpaKlJbF9K5sYyg+k8+6vNi
6ByG7bGLgwJzU4J8U3aSOey0oi+v+nYLJEZQjk/bV3mfSKuc/pM4+q46Q25Ly9b8
CElny+6noDyC5L2W0ILGJnyz4KiHKd2qmikTGWCnYkhXklORI9zhvTOQw/9sk2n4
7iM+c+jm+UeEJX0/RwKa3RwDPbhsuK8nNIPkeLndN3YZ0WNgczrywVgcQP860Pjl
b9zcO50CgYEAzRgx5KY/FVR8vf/+vhHxw/v9EwhpGkH8bYfbU2GrxlDiXWfu9wUq
5anmw75S5LnQAC7Rr4HOvqHlcVkxUUvyuCAooCQK1zhrYfTO6uSkNDui0uDnOA2p
OUca4mQFTkWIEpV8hymafXJvgGJYUsBQmRuEO9684MO2Tw30JZrsu1sCgYEAxDX/
RGHskCv3d0sWVi+LjZyN7Oyew7IOWh7Pdp1NWoPw+wNh56sH0dfm7b8Z1PvFHiYh
IhOYrFlMw88Joz49o8/xJzYTJLAmPHhgWhjRfO7pIjad7WiQ73e33KaE5CFvS6aq
7EDaKc2ka2yLXt8njbFzAINKNH4dbF66cO0QmeUCgYAZ/XPtJsgdutwn87XjfkCl
9lvmuo0fH7eOZcX8dgcflYZqEc6tjQEXo4Uzzv2QMJ/UNhqqZAfADqIV0Bi41agH
f0PGr2qURXkOK8jA6YKJf/ktQgTrDIgiUwlsIQKvHY6V1E24jyQdu4+3tD6/FdHg
5STHtX03+nUy5XDIBy7yuwKBgEUuELHIoQL4LYmHFPoQS3EDqNrHVvcC2aKWyGk2
7yYh7R5jw+pN2DT5nbdnsLDykD6gDQCZzjO+TJ2havF+qXcPgyRjIX7HCMQ7YSWD
KKGXDuX8QdSKEMgN+uq2X5ab30TaH9uqxgEFO5qQq9cocSa5USX2JHy7lp42DCBy
xW6tAoGAVQSMdMIGh2/NVWIbarWD1l8oPyevaaiFC/Asvd859Ta8M6of8/rR/rIu
ENJiZyEIpU8ewKzn+8/4p1keohkN4TSYAuAifZYL+ytDcak8P+Z5NbxHreSjN4ic
YIJ480WXdUSxebxnoemJs+HYNM7m0vugOhdb8iTwijKYSRxyPLc=
-----END RSA PRIVATE KEY-----
";

            var cipherText = @"YA5e9rEEgfV3cXT2bKwd88Ee7FwhFb7Q1AlPeHsjH2Ss9oXxP3Z2/az/Tb4dFPFfgRjNfg/Ye2w9i16MmFUD5LMhGcgRXcGCOWT8rWjq0uYi+vXH9hEAm4GfIAXgfnPtmfLtrffURd8yaEOYLu7AuaYu9THFFtC+YD55k8auixPLuCMxA9S/tIGF/CMcQSHV/MCAvNqLDTMkuHvzRgyRbKdRJlzl+mnZpWOF9cK/VPgc9kQnI+ZAgne/NKrm5bCQbe0qfrJ5+oYit6r0dTWQL/s99Z5HJKrrqDbqKXMxu3VbdWOsiux+XgVXjEMerQHWJU4hkajb623FSP3P+agjaw==";

            var s = _rsaEncryptionService.DecryptFromPem(priv, cipherText);

        }
    }
}
