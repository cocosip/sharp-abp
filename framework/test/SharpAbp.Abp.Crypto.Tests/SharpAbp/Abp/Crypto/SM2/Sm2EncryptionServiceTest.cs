using System;
using System.Text;
using Xunit;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SharpAbp.Abp.Crypto.SM2
{
    /// <summary>
    /// Unit tests for SM2 encryption service functionality including key generation, encryption, decryption, signing, and verification.
    /// </summary>
    public class Sm2EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm2EncryptionService _sm2EncryptionService;

        public Sm2EncryptionServiceTest()
        {
            _sm2EncryptionService = GetRequiredService<ISm2EncryptionService>();
        }

        /// <summary>
        /// Test basic encryption and decryption functionality with generated key pair
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var aPublic = keyPair.ExportPublicKey();
            var aPrivate = keyPair.ExportPrivateKey();
            var plainText = "hello word, it's sm";

            // Act
            var cipherText = _sm2EncryptionService.Encrypt(aPublic, plainText);
            var source = _sm2EncryptionService.Decrypt(aPrivate, cipherText);

            // Assert
            Assert.Equal(plainText, source);
        }

        /// <summary>
        /// Test decryption with third-party generated keys and ciphertext
        /// </summary>
        [Fact]
        public void Decrypt_From_Third_Test()
        {
            // Arrange
            var aPublic =
                "04348F0E358DE83E7804D2898D6B4A475BF99FCAB3D5B24D7A8701F60CBD54662D3B02106FC8E924343C089FDDE14CC7CBC3C252AF3BFA28EDB3DE7397CF171F45";
            var aPrivate = "008E47213A4E78F800949B4B3F975D5C0F54684BAD16DDA9430A1817C146F327A4";
            var data = "{\"hospitalCode\":\"123717224954206128\",\"recordNum\":\"220223033\"}";

            // Act
            var r = _sm2EncryptionService.Encrypt(aPublic, data, Encoding.UTF8).ToUpper();

            var t1 =
                "04B05D05B58E29223CAF42C6BB615929B678EEDB4B7E18DB7E74BD017CA8EE7EAFD54D06E7E6D2A8B98B106325596149374C9DF4AE5E0083D34B89912FE53C0BC60515FAAAC537E0FEE7A367F74B77936B23CE20BC51881308E1925AB821D40B0478629F23127C";
            var s1 = _sm2EncryptionService.Decrypt(aPrivate, t1);
            var s2 = _sm2EncryptionService.Decrypt(aPrivate, r, Encoding.UTF8);

            // Assert
            Assert.Equal("张三", s1);
            Assert.Equal(data, s2);
        }

        /// <summary>
        /// Test signing and signature verification functionality
        /// </summary>
        [Fact]
        public void Sign_Verify_Test()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var aPublic = keyPair.ExportPublicKey();
            var aPrivate = keyPair.ExportPrivateKey();
            var plainText = "hello word";

            // Act
            var signed = _sm2EncryptionService.Sign(aPrivate, plainText);
            var r = _sm2EncryptionService.VerifySign(aPublic, plainText, signed);

            // Assert
            Assert.True(r);
        }

        /// <summary>
        /// Test key pair generation with default curve
        /// </summary>
        [Fact]
        public void GenerateSm2KeyPair_Should_Generate_Valid_KeyPair()
        {
            // Act
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();

            // Assert
            Assert.NotNull(keyPair);
            Assert.NotNull(keyPair.Public);
            Assert.NotNull(keyPair.Private);
            Assert.False(keyPair.Public.IsPrivate);
            Assert.True(keyPair.Private.IsPrivate);
        }

        /// <summary>
        /// Test key pair generation with specific curve
        /// </summary>
        [Fact]
        public void GenerateSm2KeyPair_With_Specific_Curve_Should_Generate_Valid_KeyPair()
        {
            // Act
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair(Sm2EncryptionNames.CurveSm2p256v1);

            // Assert
            Assert.NotNull(keyPair);
            Assert.NotNull(keyPair.Public);
            Assert.NotNull(keyPair.Private);
        }

        /// <summary>
        /// Test encryption with byte array input
        /// </summary>
        [Fact]
        public void Encrypt_With_ByteArray_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");

            // Act
            var cipherTextBytes = _sm2EncryptionService.Encrypt(publicKeyBytes, plainTextBytes);

            // Assert
            Assert.NotNull(cipherTextBytes);
            Assert.True(cipherTextBytes.Length > 0);
        }

        /// <summary>
        /// Test decryption with byte array input
        /// </summary>
        [Fact]
        public void Decrypt_With_ByteArray_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");

            // Act
            var cipherTextBytes = _sm2EncryptionService.Encrypt(publicKeyBytes, plainTextBytes);
            var decryptedBytes = _sm2EncryptionService.Decrypt(privateKeyBytes, cipherTextBytes);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test encryption and decryption with C1C3C2 mode
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_With_C1C3C2_Mode_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message with C1C3C2 mode");

            // Act
            var cipherTextBytes = _sm2EncryptionService.Encrypt(publicKeyBytes, plainTextBytes, mode: Mode.C1C3C2);
            var decryptedBytes = _sm2EncryptionService.Decrypt(privateKeyBytes, cipherTextBytes, mode: Mode.C1C3C2);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test signing with byte array input
        /// </summary>
        [Fact]
        public void Sign_With_ByteArray_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message for signing");

            // Act
            var signature = _sm2EncryptionService.Sign(privateKeyBytes, plainTextBytes);

            // Assert
            Assert.NotNull(signature);
            Assert.True(signature.Length > 0);
        }

        /// <summary>
        /// Test signature verification with byte array input
        /// </summary>
        [Fact]
        public void VerifySign_With_ByteArray_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message for verification");

            // Act
            var signature = _sm2EncryptionService.Sign(privateKeyBytes, plainTextBytes);
            var isValid = _sm2EncryptionService.VerifySign(publicKeyBytes, plainTextBytes, signature);

            // Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Test signature verification with wrong signature should fail
        /// </summary>
        [Fact]
        public void VerifySign_With_Wrong_Signature_Should_Return_False()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");
            var wrongSignature = new byte[64]; // Invalid signature

            // Act & Assert
            Assert.False(_sm2EncryptionService.VerifySign(publicKeyBytes, plainTextBytes, wrongSignature));
        }

        /// <summary>
        /// Test signing and verification with custom ID
        /// </summary>
        [Fact]
        public void Sign_Verify_With_Custom_ID_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message with custom ID");
            var customId = Encoding.UTF8.GetBytes("custom_user_id");

            // Act
            var signature = _sm2EncryptionService.Sign(privateKeyBytes, plainTextBytes, id: customId);
            var isValid = _sm2EncryptionService.VerifySign(publicKeyBytes, plainTextBytes, signature, id: customId);

            // Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Test C1C2C3 to C1C3C2 conversion
        /// </summary>
        [Fact]
        public void C123ToC132_Should_Convert_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message for conversion");

            // Act
            var c1c2c3Cipher = _sm2EncryptionService.Encrypt(publicKeyBytes, plainTextBytes, mode: Mode.C1C2C3);
            var c1c3c2Cipher = _sm2EncryptionService.C123ToC132(c1c2c3Cipher);
            var decryptedBytes = _sm2EncryptionService.Decrypt(privateKeyBytes, c1c3c2Cipher, mode: Mode.C1C3C2);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test C1C3C2 to C1C2C3 conversion
        /// </summary>
        [Fact]
        public void C132ToC123_Should_Convert_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var publicKeyBytes = Hex.DecodeStrict(publicKeyHex);
            var privateKeyBytes = Hex.DecodeStrict(privateKeyHex);
            var plainTextBytes = Encoding.UTF8.GetBytes("test message for reverse conversion");

            // Act
            var c1c3c2Cipher = _sm2EncryptionService.Encrypt(publicKeyBytes, plainTextBytes, mode: Mode.C1C3C2);
            var c1c2c3Cipher = _sm2EncryptionService.C132ToC123(c1c3c2Cipher);
            var decryptedBytes = _sm2EncryptionService.Decrypt(privateKeyBytes, c1c2c3Cipher, mode: Mode.C1C2C3);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test encryption with extension method using hex string keys
        /// </summary>
        [Fact]
        public void Encrypt_Extension_With_HexString_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var plainText = "test message with extension method";

            // Act
            var cipherTextHex = _sm2EncryptionService.Encrypt(publicKeyHex, plainText);
            var decryptedText = _sm2EncryptionService.Decrypt(privateKeyHex, cipherTextHex);

            // Assert
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test signing with extension method using hex string keys
        /// </summary>
        [Fact]
        public void Sign_Extension_With_HexString_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var plainText = "test message for signing with extension";

            // Act
            var signatureHex = _sm2EncryptionService.Sign(privateKeyHex, plainText);
            var isValid = _sm2EncryptionService.VerifySign(publicKeyHex, plainText, signatureHex);

            // Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Test encryption with empty string should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_With_Empty_String_Should_Throw_Exception()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var emptyText = string.Empty;

            // Act & Assert
            Assert.Throws<DataLengthException>(() => _sm2EncryptionService.Encrypt(publicKeyHex, emptyText));
        }

        /// <summary>
        /// Test encryption with null public key should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_With_Null_PublicKey_Should_Throw_Exception()
        {
            // Arrange
            var plainText = "test message";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _sm2EncryptionService.Encrypt(null, plainText));
        }

        /// <summary>
        /// Test decryption with null private key should throw exception
        /// </summary>
        [Fact]
        public void Decrypt_With_Null_PrivateKey_Should_Throw_Exception()
        {
            // Arrange
            var cipherText = "test cipher";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _sm2EncryptionService.Decrypt(null, cipherText));
        }

        /// <summary>
        /// Test signing with null private key should throw exception
        /// </summary>
        [Fact]
        public void Sign_With_Null_PrivateKey_Should_Throw_Exception()
        {
            // Arrange
            var plainText = "test message";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _sm2EncryptionService.Sign(null, plainText));
        }

        /// <summary>
        /// Test signature verification with null public key should throw exception
        /// </summary>
        [Fact]
        public void VerifySign_With_Null_PublicKey_Should_Throw_Exception()
        {
            // Arrange
            var plainText = "test message";
            var signature = "test signature";

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _sm2EncryptionService.VerifySign(null, plainText, signature));
        }

        /// <summary>
        /// Test encryption with large text should work correctly
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_With_Large_Text_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var largeText = new string('A', 1000); // 1000 characters

            // Act
            var cipherTextHex = _sm2EncryptionService.Encrypt(publicKeyHex, largeText);
            var decryptedText = _sm2EncryptionService.Decrypt(privateKeyHex, cipherTextHex);

            // Assert
            Assert.Equal(largeText, decryptedText);
        }

        /// <summary>
        /// Test encryption with Unicode characters should work correctly
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_With_Unicode_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _sm2EncryptionService.GenerateSm2KeyPair();
            var publicKeyHex = keyPair.ExportPublicKey();
            var privateKeyHex = keyPair.ExportPrivateKey();
            var unicodeText = "测试中文字符 🚀 Test Unicode characters";

            // Act
            var cipherTextHex = _sm2EncryptionService.Encrypt(publicKeyHex, unicodeText);
            var decryptedText = _sm2EncryptionService.Decrypt(privateKeyHex, cipherTextHex);

            // Assert
            Assert.Equal(unicodeText, decryptedText);
        }
    }
}
