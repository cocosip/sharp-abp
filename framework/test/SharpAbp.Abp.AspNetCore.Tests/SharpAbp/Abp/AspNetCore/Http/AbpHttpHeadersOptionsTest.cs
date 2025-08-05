using SharpAbp.Abp.AspNetCore.Http;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class AbpHttpHeadersOptionsTest
    {
        [Fact]
        public void AbpHttpHeadersOptions_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var options = new AbpHttpHeadersOptions
            {
                RouteTranslationPrefix = "X-Route-"
            };

            // Assert
            Assert.Equal("X-Route-", options.RouteTranslationPrefix);
        }

        [Fact]
        public void AbpHttpHeadersOptions_Default_Values_Should_Be_Correct()
        {
            // Arrange
            var options = new AbpHttpHeadersOptions();

            // Assert
            Assert.Null(options.RouteTranslationPrefix);
        }
    }
}