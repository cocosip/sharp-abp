using SharpAbp.Abp.IdentityServer.Localization;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpIdentityServerDomainSharedModule)
        )]
    public class IdentityServerDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<IdentityServerDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<IdentityServerResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("SharpAbp/Abp/IdentityServer/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("SharpAbpIdentityServer", typeof(IdentityServerResource));
            });
        }
    }
}
