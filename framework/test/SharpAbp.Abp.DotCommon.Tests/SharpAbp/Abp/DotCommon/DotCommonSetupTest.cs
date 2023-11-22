using DotCommon.Serializing;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class DotCommonSetupTest : AbpDotCommonTestBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IXmlSerializer _xmlSerializer;
        public DotCommonSetupTest()
        {
            _jsonSerializer = GetRequiredService<IJsonSerializer>();
            _xmlSerializer = GetRequiredService<IXmlSerializer>();
        }

        [Fact]
        public void Setup_Inject_Object_Test()
        {
            Assert.NotNull(_jsonSerializer);
            Assert.NotNull(_xmlSerializer);
        }
    }
}
