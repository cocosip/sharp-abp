using Xunit;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    /// <summary>
    /// ASP.NET Core选项配置测试
    /// </summary>
    public class AbpTransformSecurityAspNetCoreOptionsTest
    {
        /// <summary>
        /// 测试选项默认值
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Default_Values()
        {
            // Act
            var options = new AbpTransformSecurityAspNetCoreOptions();

            // Assert
            Assert.Equal("AbpTransformSecurityIdentifier", options.TransformSecurityIdentifierName);
            Assert.NotNull(options.MiddlewareHandlers);
        }
    }
}
