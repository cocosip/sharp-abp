using System;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredential entity
    /// </summary>
    public class SecurityCredentialTest
    {
        [Fact]
        public void IsExpires_Should_Return_False_When_Expires_Is_Null()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                Expires = null
            };
            var checkTime = DateTime.Now;

            // Act
            var result = credential.IsExpires(checkTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsExpires_Should_Return_False_When_Not_Expired()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                Expires = DateTime.Now.AddMinutes(10)
            };
            var checkTime = DateTime.Now;

            // Act
            var result = credential.IsExpires(checkTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsExpires_Should_Return_True_When_Expired()
        {
            // Arrange
            var credential = new SecurityCredential
            {
                Expires = DateTime.Now.AddMinutes(-10)
            };
            var checkTime = DateTime.Now;

            // Act
            var result = credential.IsExpires(checkTime);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsExpires_Should_Return_True_When_Exactly_Expired()
        {
            // Arrange
            var expireTime = DateTime.Now;
            var credential = new SecurityCredential
            {
                Expires = expireTime
            };

            // Act
            var result = credential.IsExpires(expireTime);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void SecurityCredential_Should_Initialize_With_Default_Values()
        {
            // Act
            var credential = new SecurityCredential();

            // Assert
            Assert.Null(credential.Identifier);
            Assert.Null(credential.KeyType);
            Assert.Null(credential.BizType);
            Assert.Null(credential.PublicKey);
            Assert.Null(credential.PrivateKey);
            Assert.Null(credential.Expires);
            Assert.Equal(default(DateTime), credential.CreationTime);
        }

        [Fact]
        public void SecurityCredential_Should_Allow_Setting_All_Properties()
        {
            // Arrange
            var identifier = Guid.NewGuid().ToString("N");
            var keyType = "RSA";
            var bizType = "Login";
            var publicKey = "PublicKeyData";
            var privateKey = "PrivateKeyData";
            var expires = DateTime.Now.AddMinutes(10);
            var creationTime = DateTime.Now;

            // Act
            var credential = new SecurityCredential
            {
                Identifier = identifier,
                KeyType = keyType,
                BizType = bizType,
                PublicKey = publicKey,
                PrivateKey = privateKey,
                Expires = expires,
                CreationTime = creationTime
            };

            // Assert
            Assert.Equal(identifier, credential.Identifier);
            Assert.Equal(keyType, credential.KeyType);
            Assert.Equal(bizType, credential.BizType);
            Assert.Equal(publicKey, credential.PublicKey);
            Assert.Equal(privateKey, credential.PrivateKey);
            Assert.Equal(expires, credential.Expires);
            Assert.Equal(creationTime, credential.CreationTime);
        }

        [Theory]
        [InlineData("2023-01-01 10:00:00", "2023-01-01 09:59:59", false)]
        [InlineData("2023-01-01 10:00:00", "2023-01-01 10:00:00", true)]
        [InlineData("2023-01-01 10:00:00", "2023-01-01 10:00:01", true)]
        public void IsExpires_Should_Work_With_Various_DateTime_Scenarios(string expiresStr, string checkTimeStr, bool expectedResult)
        {
            // Arrange
            var expires = DateTime.Parse(expiresStr);
            var checkTime = DateTime.Parse(checkTimeStr);
            var credential = new SecurityCredential
            {
                Expires = expires
            };

            // Act
            var result = credential.IsExpires(checkTime);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}