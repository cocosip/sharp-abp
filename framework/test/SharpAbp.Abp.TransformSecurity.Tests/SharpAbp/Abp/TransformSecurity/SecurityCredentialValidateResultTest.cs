using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredentialValidateResult
    /// </summary>
    public class SecurityCredentialValidateResultTest
    {
        [Fact]
        public void Constructor_Should_Initialize_With_Default_Values()
        {
            // Act
            var result = new SecurityCredentialValidateResult();

            // Assert
            Assert.Equal(default(SecurityCredentialResultType), result.Result);
        }

        [Fact]
        public void Result_Property_Should_Be_Settable()
        {
            // Arrange
            var result = new SecurityCredentialValidateResult();
            var resultType = SecurityCredentialResultType.Success;

            // Act
            result.Result = resultType;

            // Assert
            Assert.Equal(resultType, result.Result);
        }

        [Theory]
        [InlineData(SecurityCredentialResultType.Success)]
        [InlineData(SecurityCredentialResultType.NotFound)]
        [InlineData(SecurityCredentialResultType.Expired)]
        public void Result_Should_Accept_All_Valid_ResultTypes(SecurityCredentialResultType resultType)
        {
            // Arrange
            var result = new SecurityCredentialValidateResult();

            // Act
            result.Result = resultType;

            // Assert
            Assert.Equal(resultType, result.Result);
        }

        [Fact]
        public void Multiple_Results_Should_Be_Independent()
        {
            // Arrange
            var result1 = new SecurityCredentialValidateResult();
            var result2 = new SecurityCredentialValidateResult();

            // Act
            result1.Result = SecurityCredentialResultType.Success;
            result2.Result = SecurityCredentialResultType.Expired;

            // Assert
            Assert.Equal(SecurityCredentialResultType.Success, result1.Result);
            Assert.Equal(SecurityCredentialResultType.Expired, result2.Result);
        }

        [Fact]
        public void Result_Should_Support_Reassignment()
        {
            // Arrange
            var result = new SecurityCredentialValidateResult
            {
                Result = SecurityCredentialResultType.Success
            };

            // Act
            result.Result = SecurityCredentialResultType.NotFound;

            // Assert
            Assert.Equal(SecurityCredentialResultType.NotFound, result.Result);
        }
    }
}