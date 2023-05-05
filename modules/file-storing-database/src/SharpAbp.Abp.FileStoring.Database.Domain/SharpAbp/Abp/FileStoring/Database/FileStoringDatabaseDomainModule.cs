using SharpAbp.Abp.FileStoring.Database.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring.Database
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpFileStoringModule),
        typeof(FileStoringDatabaseDomainSharedModule)
        )]
    public class FileStoringDatabaseDomainModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringAbstractionsOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
            return Task.CompletedTask;
        }


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    if (container.Provider.IsNullOrWhiteSpace())
                    {
                        container.UseDatabase();
                    }
                });
            });
            return Task.CompletedTask;
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(DatabaseFileProviderConsts.ProviderName, typeof(FileStoringDatabaseResource));
            return configuration;
        }
    }
}
