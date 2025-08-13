using Org.BouncyCastle.Crypto;
using SharpAbp.Abp.Crypto.RSA;
using System;
using Xunit;

namespace SharpAbp.Abp.Crypto.RSA
{
    /// <summary>
    /// Unit tests for RSA encryption service with Base64 (non-PEM) format keys
    /// </summary>
    public class RSAEncryptionServiceBase64Test : AbpCryptoTestBase
    {
        private readonly IRSAEncryptionService _rsaEncryptionService;

        public RSAEncryptionServiceBase64Test()
        {
            _rsaEncryptionService = GetRequiredService<IRSAEncryptionService>();
        }

        /// <summary>
        /// Test importing RSA public key from Base64 DER format
        /// </summary>
        [Fact]
        public void ImportPublicKey_Should_Import_Base64_DER_Format_Successfully()
        {
            // Arrange - Generate a key pair first
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();

            // Act
            var importedPublicKey = _rsaEncryptionService.ImportPublicKey(publicKeyBase64);

            // Assert
            Assert.NotNull(importedPublicKey);
            Assert.False(importedPublicKey.IsPrivate);
        }

        /// <summary>
        /// Test importing RSA private key from Base64 PKCS#1 DER format
        /// </summary>
        [Fact]
        public void ImportPrivateKey_Should_Import_Base64_PKCS1_Format_Successfully()
        {
            // Arrange - Generate a key pair first
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var privateKeyBase64 = keyPair.ExportPrivateKey();

            // Act
            var importedPrivateKey = _rsaEncryptionService.ImportPrivateKey(privateKeyBase64);

            // Assert
            Assert.NotNull(importedPrivateKey);
            Assert.True(importedPrivateKey.IsPrivate);
        }

        /// <summary>
        /// Test importing RSA private key from Base64 PKCS#8 DER format
        /// </summary>
        [Fact]
        public void ImportPrivateKeyPkcs8_Should_Import_Base64_PKCS8_Format_Successfully()
        {
            // Arrange - Generate a key pair first
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var privateKeyBase64 = keyPair.ExportPrivateKeyPkcs8();

            // Act
            var importedPrivateKey = _rsaEncryptionService.ImportPrivateKeyPkcs8(privateKeyBase64);

            // Assert
            Assert.NotNull(importedPrivateKey);
            Assert.True(importedPrivateKey.IsPrivate);
        }

        /// <summary>
        /// Test RSA encryption and decryption using Base64 format keys (PKCS#1)
        /// </summary>
        [Fact]
        public void EncryptFromBase64_And_DecryptFromBase64_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKey();
            var plainText = "Hello, RSA Base64 encryption!";

            // Act
            var encryptedText = _rsaEncryptionService.EncryptFromBase64(publicKeyBase64, plainText);
            var decryptedText = _rsaEncryptionService.DecryptFromBase64(privateKeyBase64, encryptedText);

            // Assert
            Assert.NotNull(encryptedText);
            Assert.NotEmpty(encryptedText);
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test RSA encryption and decryption using Base64 format keys (PKCS#8)
        /// </summary>
        [Fact]
        public void EncryptFromBase64_And_DecryptFromPkcs8Base64_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKeyPkcs8();
            var plainText = "Hello, RSA PKCS#8 Base64 encryption!";

            // Act
            var encryptedText = _rsaEncryptionService.EncryptFromBase64(publicKeyBase64, plainText);
            var decryptedText = _rsaEncryptionService.DecryptFromPkcs8Base64(privateKeyBase64, encryptedText);

            // Assert
            Assert.NotNull(encryptedText);
            Assert.NotEmpty(encryptedText);
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test RSA signing and verification using Base64 format keys (PKCS#1)
        /// </summary>
        [Fact]
        public void SignFromBase64_And_VerifySignFromBase64_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKey();
            var data = "Data to be signed";

            // Act
            var signature = _rsaEncryptionService.SignFromBase64(privateKeyBase64, data);
            var isValid = _rsaEncryptionService.VerifySignFromBase64(publicKeyBase64, data, signature);

            // Assert
            Assert.NotNull(signature);
            Assert.NotEmpty(signature);
            Assert.True(isValid);
        }

        /// <summary>
        /// Test RSA signing and verification using Base64 format keys (PKCS#8)
        /// </summary>
        [Fact]
        public void SignFromPkcs8Base64_And_VerifySignFromBase64_Should_Work_Correctly()
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKeyPkcs8();
            var data = "Data to be signed with PKCS#8";

            // Act
            var signature = _rsaEncryptionService.SignFromPkcs8Base64(privateKeyBase64, data);
            var isValid = _rsaEncryptionService.VerifySignFromBase64(publicKeyBase64, data, signature);

            // Assert
            Assert.NotNull(signature);
            Assert.NotEmpty(signature);
            Assert.True(isValid);
        }

        /// <summary>
        /// Test RSA signing with different algorithms using Base64 format keys
        /// </summary>
        [Theory]
        [InlineData("SHA1WITHRSA")]
        [InlineData("SHA256WITHRSA")]
        [InlineData("SHA384WITHRSA")]
        [InlineData("SHA512WITHRSA")]
        public void SignFromBase64_With_Different_Algorithms_Should_Work_Correctly(string algorithm)
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKey();
            var data = $"Data to be signed with {algorithm}";

            // Act
            var signature = _rsaEncryptionService.SignFromBase64(privateKeyBase64, data, algorithm);
            var isValid = _rsaEncryptionService.VerifySignFromBase64(publicKeyBase64, data, signature, algorithm);

            // Assert
            Assert.NotNull(signature);
            Assert.NotEmpty(signature);
            Assert.True(isValid);
        }

        /// <summary>
        /// Test RSA encryption with different padding schemes using Base64 format keys
        /// </summary>
        [Theory]
        [InlineData(RSAPaddingNames.None)]
        [InlineData(RSAPaddingNames.PKCS1Padding)]
        [InlineData(RSAPaddingNames.OAEPPadding)]
        public void EncryptFromBase64_With_Different_Padding_Should_Work_Correctly(string padding)
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKey();
            var plainText = $"Hello, RSA with {padding} padding!";

            // Act
            var encryptedText = _rsaEncryptionService.EncryptFromBase64(publicKeyBase64, plainText, padding: padding);
            var decryptedText = _rsaEncryptionService.DecryptFromBase64(privateKeyBase64, encryptedText, padding: padding);

            // Assert
            Assert.NotNull(encryptedText);
            Assert.NotEmpty(encryptedText);
            Assert.Equal(plainText, decryptedText);
        }

        /// <summary>
        /// Test that importing public key with invalid Base64 throws FormatException
        /// </summary>
        [Fact]
        public void ImportPublicKey_Should_Throw_Exception_For_Invalid_Base64()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string";

            // Act & Assert
            Assert.Throws<FormatException>(() => _rsaEncryptionService.ImportPublicKey(invalidBase64));
        }

        /// <summary>
        /// Test that importing private key with invalid Base64 throws FormatException
        /// </summary>
        [Fact]
        public void ImportPrivateKey_Should_Throw_Exception_For_Invalid_Base64()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string";

            // Act & Assert
            Assert.Throws<FormatException>(() => _rsaEncryptionService.ImportPrivateKey(invalidBase64));
        }

        /// <summary>
        /// Test that importing PKCS#8 private key with invalid Base64 throws FormatException
        /// </summary>
        [Fact]
        public void ImportPrivateKeyPkcs8_Should_Throw_Exception_For_Invalid_Base64()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string";

            // Act & Assert
            Assert.Throws<FormatException>(() => _rsaEncryptionService.ImportPrivateKeyPkcs8(invalidBase64));
        }

        /// <summary>
        /// Test that encryption with invalid public key Base64 throws exception
        /// </summary>
        [Fact]
        public void EncryptFromBase64_Should_Throw_Exception_For_Invalid_Public_Key()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string";
            var plainText = "Test data";

            // Act & Assert
            Assert.Throws<FormatException>(() => _rsaEncryptionService.EncryptFromBase64(invalidBase64, plainText));
        }

        /// <summary>
        /// Test that decryption with invalid private key Base64 throws exception
        /// </summary>
        [Fact]
        public void DecryptFromBase64_Should_Throw_Exception_For_Invalid_Private_Key()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string";
            var cipherText = "VGVzdCBkYXRh"; // Valid Base64 but not encrypted data

            // Act & Assert
            Assert.Throws<FormatException>(() => _rsaEncryptionService.DecryptFromBase64(invalidBase64, cipherText));
        }

        /// <summary>
        /// Test that signature verification fails with wrong public key
        /// </summary>
        [Fact]
        public void VerifySignFromBase64_Should_Return_False_For_Wrong_Public_Key()
        {
            // Arrange
            var keyPair1 = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var keyPair2 = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var privateKeyBase64 = keyPair1.ExportPrivateKey();
            var wrongPublicKeyBase64 = keyPair2.ExportPublicKey();
            var data = "Data to be signed";

            // Act
            var signature = _rsaEncryptionService.SignFromBase64(privateKeyBase64, data);
            var isValid = _rsaEncryptionService.VerifySignFromBase64(wrongPublicKeyBase64, data, signature);

            // Assert
            Assert.False(isValid);
        }

        /// <summary>
        /// Test that signature verification fails with tampered data
        /// </summary>
        [Fact]
        public void VerifySignFromBase64_Should_Return_False_For_Tampered_Data()
        {
            // Arrange
            var keyPair = _rsaEncryptionService.GenerateRSAKeyPair(2048);
            var publicKeyBase64 = keyPair.ExportPublicKey();
            var privateKeyBase64 = keyPair.ExportPrivateKey();
            var originalData = "Original data";
            var tamperedData = "Tampered data";

            // Act
            var signature = _rsaEncryptionService.SignFromBase64(privateKeyBase64, originalData);
            var isValid = _rsaEncryptionService.VerifySignFromBase64(publicKeyBase64, tamperedData, signature);

            // Assert
            Assert.False(isValid);
        }
    }
}