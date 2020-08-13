using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(typeof(AbpMultiTenancyModule))]
    public class AbpFileStoringModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
