using Microsoft.AspNetCore.Http;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class CookiePolicyOptionsExtensionsTest
    {
        [Fact]
        public void DisallowsSameSiteNone_Should_Return_True_For_IOS_Browsers()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X)";

            // Act
            var result = CookiePolicyOptionsExtensions.DisallowsSameSiteNone(userAgent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DisallowsSameSiteNone_Should_Return_True_For_MacOS_Safari()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.2 Safari/605.1.15";

            // Act
            var result = CookiePolicyOptionsExtensions.DisallowsSameSiteNone(userAgent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DisallowsSameSiteNone_Should_Return_True_For_Chrome_50_69()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

            // Act
            var result = CookiePolicyOptionsExtensions.DisallowsSameSiteNone(userAgent);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DisallowsSameSiteNone_Should_Return_False_For_Compatible_Browsers()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";

            // Act
            var result = CookiePolicyOptionsExtensions.DisallowsSameSiteNone(userAgent);

            // Assert
            Assert.False(result);
        }
    }
}