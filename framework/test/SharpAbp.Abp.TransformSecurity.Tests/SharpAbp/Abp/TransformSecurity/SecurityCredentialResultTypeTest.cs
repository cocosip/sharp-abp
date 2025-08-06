using System;
using Xunit;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Unit tests for SecurityCredentialResultType enum
    /// </summary>
    public class SecurityCredentialResultTypeTest
    {
        [Fact]
        public void Enum_Should_Have_Correct_Values()
        {
            // Assert
            Assert.Equal(1, (int)SecurityCredentialResultType.Success);
            Assert.Equal(2, (int)SecurityCredentialResultType.NotFound);
            Assert.Equal(3, (int)SecurityCredentialResultType.Expired);
        }

        [Fact]
        public void Enum_Should_Have_All_Expected_Members()
        {
            // Arrange
            var expectedValues = new[]
            {
                SecurityCredentialResultType.Success,
                SecurityCredentialResultType.NotFound,
                SecurityCredentialResultType.Expired
            };

            // Act
            var actualValues = Enum.GetValues<SecurityCredentialResultType>();

            // Assert
            Assert.Equal(expectedValues.Length, actualValues.Length);
            foreach (var expectedValue in expectedValues)
            {
                Assert.Contains(expectedValue, actualValues);
            }
        }

        [Theory]
        [InlineData(SecurityCredentialResultType.Success, "Success")]
        [InlineData(SecurityCredentialResultType.NotFound, "NotFound")]
        [InlineData(SecurityCredentialResultType.Expired, "Expired")]
        public void Enum_Should_Have_Correct_String_Representation(SecurityCredentialResultType value, string expectedName)
        {
            // Act
            var actualName = value.ToString();

            // Assert
            Assert.Equal(expectedName, actualName);
        }

        [Theory]
        [InlineData("Success", SecurityCredentialResultType.Success)]
        [InlineData("NotFound", SecurityCredentialResultType.NotFound)]
        [InlineData("Expired", SecurityCredentialResultType.Expired)]
        public void Enum_Should_Parse_From_String(string input, SecurityCredentialResultType expected)
        {
            // Act
            var result = Enum.Parse<SecurityCredentialResultType>(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("success", true, SecurityCredentialResultType.Success)]
        [InlineData("NOTFOUND", true, SecurityCredentialResultType.NotFound)]
        [InlineData("expired", true, SecurityCredentialResultType.Expired)]
        [InlineData("InvalidValue", false, default(SecurityCredentialResultType))]
        public void Enum_Should_TryParse_Correctly(string input, bool expectedSuccess, SecurityCredentialResultType expectedValue)
        {
            // Act
            var success = Enum.TryParse<SecurityCredentialResultType>(input, true, out var result);

            // Assert
            Assert.Equal(expectedSuccess, success);
            if (expectedSuccess)
            {
                Assert.Equal(expectedValue, result);
            }
        }

        [Fact]
        public void Enum_Should_Be_Defined_For_All_Values()
        {
            // Act & Assert
            Assert.True(Enum.IsDefined(typeof(SecurityCredentialResultType), SecurityCredentialResultType.Success));
            Assert.True(Enum.IsDefined(typeof(SecurityCredentialResultType), SecurityCredentialResultType.NotFound));
            Assert.True(Enum.IsDefined(typeof(SecurityCredentialResultType), SecurityCredentialResultType.Expired));
        }

        [Fact]
        public void Enum_Should_Not_Be_Defined_For_Invalid_Values()
        {
            // Act & Assert
            Assert.False(Enum.IsDefined(typeof(SecurityCredentialResultType), 0));
            Assert.False(Enum.IsDefined(typeof(SecurityCredentialResultType), 4));
            Assert.False(Enum.IsDefined(typeof(SecurityCredentialResultType), -1));
            Assert.False(Enum.IsDefined(typeof(SecurityCredentialResultType), 999));
        }

        [Fact]
        public void Enum_Values_Should_Be_Comparable()
        {
            // Act & Assert
            Assert.True(SecurityCredentialResultType.Success < SecurityCredentialResultType.NotFound);
            Assert.True(SecurityCredentialResultType.NotFound < SecurityCredentialResultType.Expired);
            Assert.True(SecurityCredentialResultType.Success < SecurityCredentialResultType.Expired);
        }

        [Fact]
        public void Enum_Should_Support_Equality_Comparison()
        {
            // Act & Assert
            Assert.Equal(SecurityCredentialResultType.Success, SecurityCredentialResultType.Success);
            Assert.NotEqual(SecurityCredentialResultType.Success, SecurityCredentialResultType.NotFound);
            Assert.NotEqual(SecurityCredentialResultType.NotFound, SecurityCredentialResultType.Expired);
        }

        [Fact]
        public void Enum_Should_Have_Correct_Underlying_Type()
        {
            // Act
            var underlyingType = Enum.GetUnderlyingType(typeof(SecurityCredentialResultType));

            // Assert
            Assert.Equal(typeof(int), underlyingType);
        }

        [Fact]
        public void Enum_Should_Support_GetNames()
        {
            // Act
            var names = Enum.GetNames<SecurityCredentialResultType>();

            // Assert
            Assert.Contains("Success", names);
            Assert.Contains("NotFound", names);
            Assert.Contains("Expired", names);
            Assert.Equal(3, names.Length);
        }

        [Fact]
        public void Enum_Should_Support_GetValues()
        {
            // Act
            var values = Enum.GetValues<SecurityCredentialResultType>();

            // Assert
            Assert.Contains(SecurityCredentialResultType.Success, values);
            Assert.Contains(SecurityCredentialResultType.NotFound, values);
            Assert.Contains(SecurityCredentialResultType.Expired, values);
            Assert.Equal(3, values.Length);
        }
    }
}