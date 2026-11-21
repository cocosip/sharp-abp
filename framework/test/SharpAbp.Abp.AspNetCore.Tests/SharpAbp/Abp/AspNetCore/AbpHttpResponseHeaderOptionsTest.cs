using Microsoft.Extensions.Options;
using SharpAbp.Abp.AspNetCore.Response;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [Theory]
        [InlineData(null, true)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async Task Middleware_Uses_Configured_Headers_Unless_Disabled(bool? enabled, bool appliesHeaders)
        {
            if (enabled.HasValue)
            {
                _options.IsEnabled = enabled.Value;
            }
            var middleware = new ResponseHeaderMiddleware(Options.Create(_options));
            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context, request =>
            {
                request.Response.StatusCode = StatusCodes.Status204NoContent;
                return Task.CompletedTask;
            });

            Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
            Assert.Equal(appliesHeaders, context.Response.Headers.ContainsKey("Content-Security-Policy"));
        }

    }
}
