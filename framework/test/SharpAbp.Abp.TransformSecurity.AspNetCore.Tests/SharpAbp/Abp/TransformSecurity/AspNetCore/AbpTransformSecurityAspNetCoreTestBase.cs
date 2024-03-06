using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.IO;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{

    public abstract class AbpTransformSecurityAspNetCoreTestBase : AbpWebApplicationFactoryIntegratedTest<Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var path = Path.Join(AppContext.BaseDirectory, "wwwroot");
            DirectoryHelper.CreateIfNotExists(path);
            return base.CreateHostBuilder();
        }
    }
}
