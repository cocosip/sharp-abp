using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AllApiSample.MultiTenancy;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using SharpAbp.Abp.AuditLogging;
using SharpAbp.Abp.Identity;
using SharpAbp.Abp.IdentityServer;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.DbConnectionsManagement;
using SharpAbp.MinId;

namespace AllApiSample;

[DependsOn(
    typeof(AllApiSampleDomainSharedModule),
    typeof(AuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(IdentityDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(IdentityServerDomainModule),
    typeof(AbpPermissionManagementDomainIdentityServerModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(MapTenancyManagementDomainModule),
    typeof(FileStoringManagementDomainModule),
    typeof(DbConnectionsManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(MinIdDomainModule)
    )]
public class AllApiSampleDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }
}
