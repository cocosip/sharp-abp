using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(FileStoringManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule)
        )]
    public class FileStoringManagementApplicationContractsModule : AbpModule
    {


    }
}
