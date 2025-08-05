using Xunit;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    /// <summary>
    /// 限流名称常量测试
    /// </summary>
    public class TransformSecurityRatelimitNamesTest
    {
        /// <summary>
        /// 测试限流名称常量值
        /// </summary>
        [Fact]
        public void SecurityKeyRateLimiting_Should_Have_Correct_Value()
        {
            // Assert
            Assert.Equal("SecurityKey", TransformSecurityRatelimitNames.SecurityKeyRateLimiting);
        }
    }
}
