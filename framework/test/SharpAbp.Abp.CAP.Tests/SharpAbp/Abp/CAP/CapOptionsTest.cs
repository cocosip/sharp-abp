using DotNetCore.CAP;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.CAP
{
    public class CapOptionsTest : AbpCapTestBase
    {
        private readonly CapOptions _options;
        public CapOptionsTest()
        {
            _options = GetRequiredService<IOptions<CapOptions>>().Value;
        }


        [Fact]
        public void CapOptions_Init_Value_Test()
        {
            Assert.Equal("cap-test", _options.DefaultGroup);
            Assert.Equal(3, _options.FailedRetryCount);
            Assert.Equal(120, _options.FailedRetryInterval);
        }

     
    }
}
