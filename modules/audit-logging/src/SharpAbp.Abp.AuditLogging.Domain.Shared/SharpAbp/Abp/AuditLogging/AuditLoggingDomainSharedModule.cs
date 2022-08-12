using SharpAbp.Abp.AuditLogging.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpAuditLoggingDomainSharedModule)
        )]
    public class AuditLoggingDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AuditLoggingDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<AuditLoggingResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/AuditLogging/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("SharpAbpAuditLogging", typeof(AuditLoggingResource));
            });
        }
    }
}
