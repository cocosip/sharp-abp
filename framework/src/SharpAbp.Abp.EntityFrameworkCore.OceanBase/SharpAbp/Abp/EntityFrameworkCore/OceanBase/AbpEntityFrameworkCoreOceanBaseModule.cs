using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.EntityFrameworkCore.OceanBase
{
    [DependsOn(typeof(SharpAbpEntityFrameworkCoreModule))]
    public class AbpEntityFrameworkCoreOceanBaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbContextOptions>(options => options.ConfigureDefaultConvention(
                (dbContext, builder) =>
                {
                    if (!dbContext.Database.IsOceanBase())
                    {
                        return;
                    }

                    var extension = dbContext.GetService<IDbContextOptions>().FindExtension<OceanBaseAbpOptionsExtension>()
                        ?? throw new InvalidOperationException("Configure OceanBase with AbpDbContextOptions.UseOceanBase so its compatibility mode is available to ABP conventions.");
                    builder.AddSharpAbpOceanBaseLargeTextColumnConvention(dbContext, extension.Mode);
                }));
        }
    }
}
