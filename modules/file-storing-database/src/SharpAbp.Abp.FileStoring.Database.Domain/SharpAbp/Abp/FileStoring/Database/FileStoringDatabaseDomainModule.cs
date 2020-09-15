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
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    if (container.ProviderType == null)
                    {
                        container.UseDatabase();
                    }
                });
            });
        }
    }
}
