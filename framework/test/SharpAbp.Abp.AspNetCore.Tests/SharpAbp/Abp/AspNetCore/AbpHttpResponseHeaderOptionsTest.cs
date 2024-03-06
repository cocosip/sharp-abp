using Microsoft.Extensions.Options;
using SharpAbp.Abp.AspNetCore.Response;
using Xunit;

namespace SharpAbp.Abp.AspNetCore
{

    public class AbpHttpResponseHeaderOptionsTest : SharpAbpAspNetCoreTestBase
    {
        private readonly AbpHttpResponseHeaderOptions _options;
        public AbpHttpResponseHeaderOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpHttpResponseHeaderOptions>>().Value;
        }

        [Fact]
        public void Get_Test()
        {
            Assert.Equal(3, _options.Headers.Count);
            var v1 = _options.Headers["Content-Security-Policy"];
            Assert.Equal(5, v1.Count);
        }

    }
}
