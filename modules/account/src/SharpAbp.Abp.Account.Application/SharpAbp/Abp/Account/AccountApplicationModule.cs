﻿using SharpAbp.Abp.Identity;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.Account
{
    [DependsOn(
        typeof(AccountApplicationContractsModule),
        typeof(IdentityApplicationModule),
        typeof(AbpUiNavigationModule),
        typeof(AbpEmailingModule)
    )]
    public class AccountApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AccountApplicationModule>();
            });

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<AbpAccountApplicationModuleAutoMapperProfile>(validate: true);
            });

            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].Urls[AccountUrlNames.PasswordReset] = "Account/ResetPassword";
            });

            return Task.CompletedTask;
        }
    }
}
