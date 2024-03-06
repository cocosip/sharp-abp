using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.AspNetCore.FrontHost;
using System.IO;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.AspNetCore
{
    public class AbpFrontHostOptionsTest : SharpAbpAspNetCoreTestBase
    {
        private readonly AbpFrontHostOptions _options;
        private readonly IWebHostEnvironment _webHostEnv;
        public AbpFrontHostOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpFrontHostOptions>>().Value;
            _webHostEnv = GetRequiredService<IWebHostEnvironment>();
        }

        [Fact]
        public void Get_Test()
        {
            Assert.Single(_options.Apps);
            var app = _options.Apps.First();
            Assert.Equal("AdminWeb", app.Name);
            Assert.Equal(Path.Join(_webHostEnv.ContentRootPath, "admin-web"), app.RootPath);
            Assert.Single(app.Pages);
            var page = app.Pages.First();
            Assert.Equal("admin/{**all}", page.Route);
            Assert.Equal("text/html", page.ContentType);
            Assert.Equal(Path.Join(_webHostEnv.ContentRootPath, "admin-web", "index.html"), page.Path);
        }

    }
}
