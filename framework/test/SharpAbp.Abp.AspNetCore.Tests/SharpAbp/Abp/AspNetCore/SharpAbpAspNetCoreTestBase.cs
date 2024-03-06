using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.IO;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.IO;

namespace SharpAbp.Abp.AspNetCore
{
    public abstract class SharpAbpAspNetCoreTestBase : AbpWebApplicationFactoryIntegratedTest<Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var path = Path.Join(AppContext.BaseDirectory, "wwwroot");
            DirectoryHelper.CreateIfNotExists(path);
            return base.CreateHostBuilder();
        }
    }
}
