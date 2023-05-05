using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.Account.Localization;
using SharpAbp.Abp.Account.Web.Pages.Account;
using SharpAbp.Abp.Account.Web.ProfileManagement;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.Account.Web
{
    [DependsOn(
        typeof(AccountApplicationContractsModule),
        typeof(AbpIdentityAspNetCoreModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpExceptionHandlingModule)
        )]
    public class AccountWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(AccountResource), typeof(AccountWebModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AccountWebModule).Assembly);
            });
            return Task.CompletedTask;
        }


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AccountWebModule>();
            });

            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new AbpAccountUserMenuContributor());
            });

            Configure<AbpToolbarOptions>(options =>
            {
                options.Contributors.Add(new AccountModuleToolbarContributor());
            });

            ConfigureProfileManagementPage();

            context.Services.AddAutoMapperObjectMapper<AccountWebModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<AbpAccountWebAutoMapperProfile>();
            });
            return Task.CompletedTask;
        }


        private void ConfigureProfileManagementPage()
        {
            Configure<RazorPagesOptions>(options =>
            {
                options.Conventions.AuthorizePage("/Account/Manage");
            });

            Configure<ProfileManagementPageOptions>(options =>
            {
                options.Contributors.Add(new AccountProfileManagementPageContributor());
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options.ScriptBundles
                    .Configure(typeof(ManageModel).FullName,
                        configuration =>
                        {
                            configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/Password/Default.js");
                            configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.js");
                        });
            });

        }
    }
}
