﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;

namespace AllApiSample;

[DependsOn(
    typeof(AllApiSampleApplicationContractsModule)
    //typeof(AbpAccountHttpApiClientModule),
    //typeof(AbpIdentityHttpApiClientModule),
    //typeof(AbpPermissionManagementHttpApiClientModule),
    //typeof(AbpTenantManagementHttpApiClientModule),
    //typeof(AbpFeatureManagementHttpApiClientModule),
    //typeof(AbpSettingManagementHttpApiClientModule)
)]
public class AllApiSampleHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AllApiSampleApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AllApiSampleHttpApiClientModule>();
        });
    }
}
