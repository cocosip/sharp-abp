using SharpAbp.Abp.DotCommon.Performance;
using Xunit;

namespace SharpAbp.Abp.DotCommon
{
    public class DefaultLogContextTextServiceTest
    {
        [Fact]
        public void GetLogContextText_Should_Use_Name_And_Key()
        {
            var service = new DefaultLogContextTextService();

            var text = service.GetLogContextText("orders", "tenant-1");

            Assert.Equal("orders-tenant-1 logging", text);
        }
    }
}
