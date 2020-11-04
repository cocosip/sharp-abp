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
        public void Get_Configured_Option_Test()
        {
            Assert.Equal("test-group", _options.DefaultGroup);
            Assert.Equal("2.0", _options.Version);
            Assert.Equal(3, _options.ConsumerThreadCount);
            Assert.Equal(10, _options.FailedRetryCount);
            Assert.Equal(30, _options.FailedRetryInterval);
            Assert.Equal(3600, _options.SucceedMessageExpiredAfter);
        }

    }
}
