using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class FasterLogNameAttributeTests
    {
        [Fact]
        public void GetLogName_WithAttribute_ReturnsAttributeName()
        {
            var name = FasterLogNameAttribute.GetLogName<FasterTestEntry>();
            Assert.Equal("faster-test-entry", name);
        }

        [Fact]
        public void GetLogName_WithoutAttribute_ReturnsFullTypeName()
        {
            var name = FasterLogNameAttribute.GetLogName<NoAttributeClass>();
            Assert.Equal(typeof(NoAttributeClass).FullName, name);
        }

        [Fact]
        public void GetLogName_DefaultFasterLog_ReturnsDefault()
        {
            var name = FasterLogNameAttribute.GetLogName<DefaultFasterLog>();
            Assert.Equal(DefaultFasterLog.Name, name);
        }

        private class NoAttributeClass { }
    }
}
