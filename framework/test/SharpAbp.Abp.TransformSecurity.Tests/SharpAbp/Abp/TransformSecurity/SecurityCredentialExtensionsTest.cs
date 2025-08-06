using Org.BouncyCastle.Crypto.Engines;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredentialExtensions
    /// </summary>
    public class SecurityCredentialExtensionsTest
    {
        [Fact]
        public void IsRSA_Should_Return_True_For_RSA_KeyType()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                KeyType = "RSA"
            };

            // Act
            var result = credential.IsRSA();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsRSA_Should_Return_False_For_Non_RSA_KeyType()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                KeyType = "SM2"
            };

            // Act
            var result = credential.IsRSA();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSM2_Should_Return_True_For_SM2_KeyType()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                KeyType = "SM2"
            };

            // Act
            var result = credential.IsSM2();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSM2_Should_Return_False_For_Non_SM2_KeyType()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                KeyType = "RSA"
            };

            // Act
            var result = credential.IsSM2();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SetReferenceId_And_GetReferenceId_Should_Work()
        {
            // Arrange
            var credential = new SecurityCredential();
            var referenceId = "TestReferenceId";

            // Act
            credential.SetReferenceId(referenceId);
            var result = credential.GetReferenceId();

            // Assert
            Assert.Equal(referenceId, result);
        }

        [Fact]
        public void GetReferenceId_Should_Return_Empty_String_When_Not_Set()
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            var result = credential.GetReferenceId();

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void SetRSAKeySize_And_GetRSAKeySize_Should_Work()
        {
            // Arrange
            var credential = new SecurityCredential();
            var keySize = 4096;

            // Act
            credential.SetRSAKeySize(keySize);
            var result = credential.GetRSAKeySize();

            // Assert
            Assert.Equal(keySize, result);
        }

        [Fact]
        public void GetRSAKeySize_Should_Return_Default_2048_When_Not_Set()
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            var result = credential.GetRSAKeySize();

            // Assert
            Assert.Equal(2048, result);
        }

        [Fact]
        public void SetRSAPadding_And_GetRSAPadding_Should_Work()
        {
            // Arrange
            var credential = new SecurityCredential();
            var padding = RSAPaddingNames.PKCS1Padding;

            // Act
            credential.SetRSAPadding(padding);
            var result = credential.GetRSAPadding();

            // Assert
            Assert.Equal(padding, result);
        }

        [Fact]
        public void GetRSAPadding_Should_Return_Default_None_When_Not_Set()
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            var result = credential.GetRSAPadding();

            // Assert
            Assert.Equal(RSAPaddingNames.None, result);
        }

        [Fact]
        public void SetSM2Curve_And_GetSM2Curve_Should_Work()
        {
            // Arrange
            var credential = new SecurityCredential();
            var curve = Sm2EncryptionNames.CurveSm2p256v1;

            // Act
            credential.SetSM2Curve(curve);
            var result = credential.GetSM2Curve();

            // Assert
            Assert.Equal(curve, result);
        }

        [Fact]
        public void GetSM2Curve_Should_Return_Default_When_Not_Set()
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            var result = credential.GetSM2Curve();

            // Assert
            Assert.Equal(Sm2EncryptionNames.CurveSm2p256v1, result);
        }

        [Fact]
        public void SetSM2Mode_And_GetSM2Mode_Should_Work()
        {
            // Arrange
            var credential = new SecurityCredential();
            var mode = SM2Engine.Mode.C1C3C2;

            // Act
            credential.SetSM2Mode(mode);
            var result = credential.GetSM2Mode();

            // Assert
            Assert.Equal(mode, result);
        }

        [Fact]
        public void GetSM2Mode_Should_Return_Default_C1C2C3_When_Not_Set()
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            var result = credential.GetSM2Mode();

            // Assert
            Assert.Equal(SM2Engine.Mode.C1C2C3, result);
        }

        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public void RSAKeySize_Should_Support_Various_Sizes(int keySize)
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            credential.SetRSAKeySize(keySize);
            var result = credential.GetRSAKeySize();

            // Assert
            Assert.Equal(keySize, result);
        }

        [Theory]
        [InlineData(RSAPaddingNames.None)]
        [InlineData(RSAPaddingNames.PKCS1Padding)]
        [InlineData(RSAPaddingNames.OAEPPadding)]
        public void RSAPadding_Should_Support_Various_Paddings(string padding)
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            credential.SetRSAPadding(padding);
            var result = credential.GetRSAPadding();

            // Assert
            Assert.Equal(padding, result);
        }

        [Theory]
        [InlineData(SM2Engine.Mode.C1C2C3)]
        [InlineData(SM2Engine.Mode.C1C3C2)]
        public void SM2Mode_Should_Support_Various_Modes(SM2Engine.Mode mode)
        {
            // Arrange
            var credential = new SecurityCredential();

            // Act
            credential.SetSM2Mode(mode);
            var result = credential.GetSM2Mode();

            // Assert
            Assert.Equal(mode, result);
        }

        [Fact]
        public void Extensions_Should_Chain_Properly()
        {
            // Arrange
            var credential = new SecurityCredential();
            var referenceId = "TestRef";
            var keySize = 4096;
            var padding = RSAPaddingNames.PKCS1Padding;

            // Act
            var result = credential
                .SetReferenceId(referenceId)
                .SetRSAKeySize(keySize)
                .SetRSAPadding(padding);

            // Assert
            Assert.Same(credential, result);
            Assert.Equal(referenceId, credential.GetReferenceId());
            Assert.Equal(keySize, credential.GetRSAKeySize());
            Assert.Equal(padding, credential.GetRSAPadding());
        }
    }
}