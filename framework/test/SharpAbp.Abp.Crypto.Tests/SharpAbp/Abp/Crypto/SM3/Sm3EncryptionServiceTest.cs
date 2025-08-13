using System;
using System.Text;
using Xunit;
using Org.BouncyCastle.Utilities.Encoders;

namespace SharpAbp.Abp.Crypto.SM3
{
    /// <summary>
    /// Unit tests for SM3 encryption service functionality including hash generation with various inputs and encodings.
    /// </summary>
    public class Sm3EncryptionServiceTest : AbpCryptoTestBase
    {
        private readonly ISm3EncryptionService _sm3EncryptionService;

        public Sm3EncryptionServiceTest()
        {
            _sm3EncryptionService = GetRequiredService<ISm3EncryptionService>();
        }

        /// <summary>
        /// Test basic hash generation with string input
        /// </summary>
        [Fact]
        public void GetHash_Test()
        {
            // Arrange
            var plainText = "hello sm";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(plainText);
            
            // Assert
            Assert.NotEmpty(hash);
        }

        /// <summary>
        /// Test hash generation with byte array input
        /// </summary>
        [Fact]
        public void GetHash_With_ByteArray_Should_Work_Correctly()
        {
            // Arrange
            var plainTextBytes = Encoding.UTF8.GetBytes("test message");
            
            // Act
            var hashBytes = _sm3EncryptionService.GetHash(plainTextBytes);
            
            // Assert
            Assert.NotNull(hashBytes);
            Assert.Equal(32, hashBytes.Length); // SM3 produces 256-bit (32-byte) hash
        }

        /// <summary>
        /// Test hash generation with empty string
        /// </summary>
        [Fact]
        public void GetHash_With_Empty_String_Should_Work()
        {
            // Arrange
            var emptyString = string.Empty;
            
            // Act
            var hash = _sm3EncryptionService.GetHash(emptyString);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length); // Hex string should be 64 characters for 32-byte hash
        }

        /// <summary>
        /// Test hash generation with empty byte array
        /// </summary>
        [Fact]
        public void GetHash_With_Empty_ByteArray_Should_Work()
        {
            // Arrange
            var emptyBytes = new byte[0];
            
            // Act
            var hashBytes = _sm3EncryptionService.GetHash(emptyBytes);
            
            // Assert
            Assert.NotNull(hashBytes);
            Assert.Equal(32, hashBytes.Length);
        }

        /// <summary>
        /// Test hash generation with null input should throw exception
        /// </summary>
        [Fact]
        public void GetHash_With_Null_ByteArray_Should_Throw_Exception()
        {
            // Arrange
            byte[] nullBytes = null;
            
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _sm3EncryptionService.GetHash(nullBytes));
        }

        /// <summary>
        /// Test hash generation with null string should throw exception
        /// </summary>
        [Fact]
        public void GetHash_With_Null_String_Should_Throw_Exception()
        {
            // Arrange
            string nullString = null;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _sm3EncryptionService.GetHash(nullString));
        }

        /// <summary>
        /// Test hash generation with different encodings
        /// </summary>
        [Fact]
        public void GetHash_With_Different_Encodings_Should_Work()
        {
            // Arrange
            var plainText = "测试中文";
            
            // Act
            var hashUtf8 = _sm3EncryptionService.GetHash(plainText, Encoding.UTF8);
            var hashUtf16 = _sm3EncryptionService.GetHash(plainText, Encoding.Unicode);
            var hashAscii = _sm3EncryptionService.GetHash("test", Encoding.ASCII);
            
            // Assert
            Assert.NotEmpty(hashUtf8);
            Assert.NotEmpty(hashUtf16);
            Assert.NotEmpty(hashAscii);
            Assert.NotEqual(hashUtf8, hashUtf16); // Different encodings should produce different hashes
        }

        /// <summary>
        /// Test hash consistency - same input should produce same hash
        /// </summary>
        [Fact]
        public void GetHash_Should_Be_Consistent()
        {
            // Arrange
            var plainText = "consistency test";
            
            // Act
            var hash1 = _sm3EncryptionService.GetHash(plainText);
            var hash2 = _sm3EncryptionService.GetHash(plainText);
            
            // Assert
            Assert.Equal(hash1, hash2);
        }

        /// <summary>
        /// Test hash generation with large text
        /// </summary>
        [Fact]
        public void GetHash_With_Large_Text_Should_Work()
        {
            // Arrange
            var largeText = new string('A', 10000); // 10KB of 'A' characters
            
            // Act
            var hash = _sm3EncryptionService.GetHash(largeText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test hash generation with Unicode characters
        /// </summary>
        [Fact]
        public void GetHash_With_Unicode_Should_Work()
        {
            // Arrange
            var unicodeText = "测试中文字符 🚀 Test Unicode characters";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(unicodeText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test hash generation with special characters
        /// </summary>
        [Fact]
        public void GetHash_With_Special_Characters_Should_Work()
        {
            // Arrange
            var specialText = "!@#$%^&*()_+-=[]{}|;':,.<>?";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(specialText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test hash generation with numeric strings
        /// </summary>
        [Fact]
        public void GetHash_With_Numeric_String_Should_Work()
        {
            // Arrange
            var numericText = "1234567890";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(numericText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test hash generation with whitespace characters
        /// </summary>
        [Fact]
        public void GetHash_With_Whitespace_Should_Work()
        {
            // Arrange
            var whitespaceText = "   \t\n\r   ";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(whitespaceText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test that different inputs produce different hashes
        /// </summary>
        [Fact]
        public void GetHash_Different_Inputs_Should_Produce_Different_Hashes()
        {
            // Arrange
            var text1 = "hello world";
            var text2 = "hello world!";
            
            // Act
            var hash1 = _sm3EncryptionService.GetHash(text1);
            var hash2 = _sm3EncryptionService.GetHash(text2);
            
            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        /// <summary>
        /// Test hash output format is valid hexadecimal
        /// </summary>
        [Fact]
        public void GetHash_Output_Should_Be_Valid_Hex()
        {
            // Arrange
            var plainText = "test hex output";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(plainText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
            
            // Verify it's valid hex by trying to decode it
            var decodedBytes = Hex.DecodeStrict(hash);
            Assert.Equal(32, decodedBytes.Length);
        }

        /// <summary>
        /// Test hash generation with binary data
        /// </summary>
        [Fact]
        public void GetHash_With_Binary_Data_Should_Work()
        {
            // Arrange
            var binaryData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD, 0xFC };
            
            // Act
            var hashBytes = _sm3EncryptionService.GetHash(binaryData);
            
            // Assert
            Assert.NotNull(hashBytes);
            Assert.Equal(32, hashBytes.Length);
        }

        /// <summary>
        /// Test hash generation with very long byte array
        /// </summary>
        [Fact]
        public void GetHash_With_Large_ByteArray_Should_Work()
        {
            // Arrange
            var largeData = new byte[100000]; // 100KB of data
            for (int i = 0; i < largeData.Length; i++)
            {
                largeData[i] = (byte)(i % 256);
            }
            
            // Act
            var hashBytes = _sm3EncryptionService.GetHash(largeData);
            
            // Assert
            Assert.NotNull(hashBytes);
            Assert.Equal(32, hashBytes.Length);
        }

        /// <summary>
        /// Test extension method with default UTF8 encoding
        /// </summary>
        [Fact]
        public void GetHash_Extension_With_Default_Encoding_Should_Work()
        {
            // Arrange
            var plainText = "test extension method";
            
            // Act
            var hash = _sm3EncryptionService.GetHash(plainText);
            
            // Assert
            Assert.NotEmpty(hash);
            Assert.Equal(64, hash.Length);
        }

        /// <summary>
        /// Test that byte array and string methods produce consistent results
        /// </summary>
        [Fact]
        public void GetHash_ByteArray_And_String_Should_Be_Consistent()
        {
            // Arrange
            var plainText = "consistency check";
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            
            // Act
            var hashFromString = _sm3EncryptionService.GetHash(plainText);
            var hashBytesFromByteArray = _sm3EncryptionService.GetHash(plainTextBytes);
            var hashFromByteArray = Hex.ToHexString(hashBytesFromByteArray);
            
            // Assert
            Assert.Equal(hashFromString.ToUpper(), hashFromByteArray.ToUpper());
        }
    }
}
