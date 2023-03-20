using System;
using SharpAbp.Abp.FileStoring.Database.Localization;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

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
            Configure<AbpFileStoringAbstractionsOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
        }


        public override void ConfigureServices(ServiceConfigurationContext context)
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
        }

        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(DatabaseFileProviderConsts.ProviderName, typeof(FileStoringDatabaseResource));
            return configuration;
        }
    }
}
