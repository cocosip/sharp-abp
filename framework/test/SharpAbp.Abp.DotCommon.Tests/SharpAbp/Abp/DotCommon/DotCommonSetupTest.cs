using DotCommon.Serializing;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class DotCommonSetupTest : AbpDotCommonTestBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IXmlSerializer _xmlSerializer;
        private readonly IObjectSerializer _objectSerializer;
        public DotCommonSetupTest()
        {
            _jsonSerializer = GetRequiredService<IJsonSerializer>();
            _xmlSerializer = GetRequiredService<IXmlSerializer>();
            _objectSerializer = GetRequiredService<IObjectSerializer>();
        }

        [Fact]
        public void Setup_Inject_Object_Test()
        {
            Assert.NotNull(_jsonSerializer);
            Assert.NotNull(_xmlSerializer);
            Assert.NotNull(_objectSerializer);
        }
    }
}
