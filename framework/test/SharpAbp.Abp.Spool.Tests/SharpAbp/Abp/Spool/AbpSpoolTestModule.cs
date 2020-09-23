using Spool;
using System;
using System.IO;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Spool
{
    [DependsOn(
       typeof(AbpSpoolModule),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
       )]
    public class AbpSpoolTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var poolPath = Path.Combine(AppContext.BaseDirectory, "default1");
            Configure<SpoolOption>(options =>
            {
                options.DefaultPool = "default";
                options.FilePools.Add(new FilePoolDescriptor()
                {
                    Name = "default",
                    Path = poolPath,
                    EnableAutoReturn = false
                });
            });

            if (Directory.Exists(poolPath))
            {
                Directory.Delete(poolPath, true);
            }

        }


    }
}
