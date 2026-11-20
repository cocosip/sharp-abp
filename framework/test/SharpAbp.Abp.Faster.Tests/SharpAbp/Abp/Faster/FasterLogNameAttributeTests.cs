using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class FasterLogNameAttributeTests
    {
        [Fact]
        public void Unnamed_Type_Requires_An_Explicit_Name()
        {
            Assert.Throws<System.ArgumentException>(() => FasterLogNameAttribute.GetLogName(typeof(Generic<>).GetGenericArguments()[0]));
        }

        private class Generic<T> { }

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
