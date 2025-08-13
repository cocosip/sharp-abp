using System;
using System.Text;
using Xunit;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto;

namespace SharpAbp.Abp.Crypto.SM4
{
    /// <summary>
    /// Unit tests for SM4 encryption service functionality including encryption, decryption with various modes and paddings.
    /// </summary>
    public class Sm4EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm4EncryptionService _sm4EncryptionService;

        public Sm4EncryptionServiceTest()
        {
            _sm4EncryptionService = GetRequiredService<ISm4EncryptionService>();
        }

        /// <summary>
        /// Test basic encryption and decryption with ECB mode and PKCS7 padding
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_Test()
        {
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));

            var plainText = "0123456789HelloWorld1234567890ABCDEF";

            var cipherText = _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            var source = _sm4EncryptionService.Decrypt(cipherText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            Assert.Equal(plainText, source);
        }

        /// <summary>
        /// Test encryption and decryption with byte arrays using ECB mode
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_ByteArray_ECB_Should_Work()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var plainTextBytes = Encoding.UTF8.GetBytes("Hello World Test Message");
            var ivBytes = new byte[16]; // IV not used in ECB mode

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(plainTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedBytes = _sm4EncryptionService.Decrypt(cipherTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test encryption and decryption with CBC mode
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_CBC_Mode_Should_Work()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = Encoding.UTF8.GetBytes("1234567890abcdef");
            var plainTextBytes = Encoding.UTF8.GetBytes("Test message for CBC mode encryption");

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(plainTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeCBC, Sm4EncryptionNames.PKCS7Padding);
            var decryptedBytes = _sm4EncryptionService.Decrypt(cipherTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeCBC, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test encryption and decryption with NoPadding
        /// </summary>
        [Fact]
        public void Encrypt_Decrypt_NoPadding_Should_Work()
        {
            // Arrange - message must be exactly 16 bytes for NoPadding
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = new byte[16];
            var plainTextBytes = Encoding.UTF8.GetBytes("1234567890123456"); // Exactly 16 bytes

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(plainTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.NoPadding);
            var decryptedBytes = _sm4EncryptionService.Decrypt(cipherTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.NoPadding);

            // Assert
            Assert.Equal(plainTextBytes, decryptedBytes);
        }

        /// <summary>
        /// Test encryption with null plaintext should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_With_Null_PlainText_Should_Throw_Exception()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sm4EncryptionService.Encrypt(null, keyBytes, ivBytes));
        }

        /// <summary>
        /// Test encryption with null key should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_With_Null_Key_Should_Throw_Exception()
        {
            // Arrange
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sm4EncryptionService.Encrypt(plainTextBytes, null, ivBytes));
        }

        /// <summary>
        /// Test encryption with invalid key length should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_With_Invalid_Key_Length_Should_Throw_Exception()
        {
            // Arrange
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");
            var invalidKeyBytes = Encoding.UTF8.GetBytes("shortkey"); // Less than 16 bytes
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sm4EncryptionService.Encrypt(plainTextBytes, invalidKeyBytes, ivBytes));
        }

        /// <summary>
        /// Test decryption with null ciphertext should throw exception
        /// </summary>
        [Fact]
        public void Decrypt_With_Null_CipherText_Should_Throw_Exception()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sm4EncryptionService.Decrypt(null, keyBytes, ivBytes));
        }

        /// <summary>
        /// Test decryption with null key should throw exception
        /// </summary>
        [Fact]
        public void Decrypt_With_Null_Key_Should_Throw_Exception()
        {
            // Arrange
            var cipherTextBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sm4EncryptionService.Decrypt(cipherTextBytes, null, ivBytes));
        }

        /// <summary>
        /// Test decryption with invalid key length should throw exception
        /// </summary>
        [Fact]
        public void Decrypt_With_Invalid_Key_Length_Should_Throw_Exception()
        {
            // Arrange
            var cipherTextBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            var invalidKeyBytes = Encoding.UTF8.GetBytes("shortkey"); // Less than 16 bytes
            var ivBytes = new byte[16];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sm4EncryptionService.Decrypt(cipherTextBytes, invalidKeyBytes, ivBytes));
        }

        /// <summary>
        /// Test CBC mode with invalid IV length should throw exception
        /// </summary>
        [Fact]
        public void Encrypt_CBC_With_Invalid_IV_Length_Should_Throw_Exception()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");
            var invalidIvBytes = new byte[8]; // Less than 16 bytes

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sm4EncryptionService.Encrypt(plainTextBytes, keyBytes, invalidIvBytes, Sm4EncryptionNames.ModeCBC, Sm4EncryptionNames.PKCS7Padding));
        }

        /// <summary>
        /// Test extension method encryption with hex string keys
        /// </summary>
        [Fact]
        public void Encrypt_Extension_With_HexString_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var plainText = "Test message with extension method";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test encryption with different encodings
        /// </summary>
        [Fact]
        public void Encrypt_With_Different_Encodings_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var plainText = "Test message with UTF8 encoding";

            // Act - Test with UTF8 encoding
            var cipherTextHex = _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding, Encoding.UTF8);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding, Encoding.UTF8);

            // Assert
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test encryption with Unicode characters
        /// </summary>
        [Fact]
        public void Encrypt_With_Unicode_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var unicodeText = "测试中文字符 🚀 Test Unicode characters";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(unicodeText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(unicodeText, decryptedText);
        }

        /// <summary>
        /// Test encryption with empty string
        /// </summary>
        [Fact]
        public void Encrypt_With_Empty_String_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var emptyText = "";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(emptyText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(emptyText, decryptedText);
        }

        /// <summary>
        /// Test encryption with large text
        /// </summary>
        [Fact]
        public void Encrypt_With_Large_Text_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var largeText = new string('A', 1000); // 1000 characters

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(largeText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(largeText, decryptedText);
        }

        /// <summary>
        /// Test encryption with special characters
        /// </summary>
        [Fact]
        public void Encrypt_With_Special_Characters_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var specialText = "!@#$%^&*()_+-=[]{}|;':,.<>?";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(specialText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(specialText, decryptedText);
        }

        /// <summary>
        /// Test encryption with numeric data
        /// </summary>
        [Fact]
        public void Encrypt_With_Numeric_Data_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var numericText = "1234567890.123456789";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(numericText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(numericText, decryptedText);
        }

        /// <summary>
        /// Test encryption with whitespace characters
        /// </summary>
        [Fact]
        public void Encrypt_With_Whitespace_Should_Work()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var whitespaceText = "  \t\n\r  ";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(whitespaceText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedText = _sm4EncryptionService.Decrypt(cipherTextHex, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(whitespaceText, decryptedText);
        }

        /// <summary>
        /// Test that different inputs produce different cipher texts
        /// </summary>
        [Fact]
        public void Encrypt_Different_Inputs_Should_Produce_Different_Outputs()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var plainText1 = "message1";
            var plainText2 = "message2";

            // Act
            var cipherText1 = _sm4EncryptionService.Encrypt(plainText1, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var cipherText2 = _sm4EncryptionService.Encrypt(plainText2, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.NotEqual(cipherText1, cipherText2);
        }

        /// <summary>
        /// Test that cipher text output format is valid hexadecimal
        /// </summary>
        [Fact]
        public void Encrypt_Output_Should_Be_Valid_Hex()
        {
            // Arrange
            var keyHex = Hex.ToHexString(Encoding.UTF8.GetBytes("abcdefghijklmnop"));
            var ivHex = Hex.ToHexString(Encoding.UTF8.GetBytes("1234567890abcdef"));
            var plainText = "test message";

            // Act
            var cipherTextHex = _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.NotEmpty(cipherTextHex);
            Assert.True(cipherTextHex.Length % 2 == 0); // Hex strings should have even length
            
            // Verify it's valid hex by trying to decode it
            var decodedBytes = Hex.DecodeStrict(cipherTextHex);
            Assert.NotEmpty(decodedBytes);
        }

        /// <summary>
        /// Test encryption with binary data
        /// </summary>
        [Fact]
        public void Encrypt_With_Binary_Data_Should_Work()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = new byte[16];
            var binaryData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD, 0xFC, 0x80, 0x7F, 0x40, 0x3F, 0x20, 0x1F, 0x10, 0x0F };

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(binaryData, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedBytes = _sm4EncryptionService.Decrypt(cipherTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(binaryData, decryptedBytes);
        }

        /// <summary>
        /// Test encryption with large byte array
        /// </summary>
        [Fact]
        public void Encrypt_With_Large_ByteArray_Should_Work()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var ivBytes = new byte[16];
            var largeData = new byte[10000]; // 10KB of data
            new Random(42).NextBytes(largeData); // Fill with random data using fixed seed for reproducibility

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(largeData, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var decryptedBytes = _sm4EncryptionService.Decrypt(cipherTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(largeData, decryptedBytes);
        }

        /// <summary>
        /// Test consistency between byte array and string methods
        /// </summary>
        [Fact]
        public void ByteArray_And_String_Methods_Should_Be_Consistent()
        {
            // Arrange
            var keyBytes = Encoding.UTF8.GetBytes("abcdefghijklmnop");
            var keyHex = Hex.ToHexString(keyBytes);
            var ivBytes = new byte[16];
            var ivHex = Hex.ToHexString(ivBytes);
            var plainText = "consistency test message";
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Act
            var cipherTextBytes = _sm4EncryptionService.Encrypt(plainTextBytes, keyBytes, ivBytes, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);
            var cipherTextHex1 = Hex.ToHexString(cipherTextBytes);
            var cipherTextHex2 = _sm4EncryptionService.Encrypt(plainText, keyHex, ivHex, Sm4EncryptionNames.ModeECB, Sm4EncryptionNames.PKCS7Padding);

            // Assert
            Assert.Equal(cipherTextHex1, cipherTextHex2);
        }
    }
}
