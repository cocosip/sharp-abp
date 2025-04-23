using DotCommon.Json;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class DotCommonSetupTest : AbpDotCommonTestBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        public DotCommonSetupTest()
        {
            _jsonSerializer = GetRequiredService<IJsonSerializer>();
        }

        [Fact]
        public void Setup_Inject_Object_Test()
        {
            Assert.NotNull(_jsonSerializer);
        }
    }
}
