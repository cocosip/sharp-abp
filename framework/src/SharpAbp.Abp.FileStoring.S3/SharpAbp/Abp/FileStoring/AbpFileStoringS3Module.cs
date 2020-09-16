using SharpAbp.Abp.AmazonS3.KS3;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
      typeof(AbpFileStoringModule),
      typeof(AbpAmazonS3KS3Module)
      )]
    public class AbpFileStoringS3Module : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFileStoringOptions>(c =>
            {
                //c.Providers.TryAdd(new FileProviderConfiguration(typeof(S3FileProvider), typeof(S3FileNamingNormalizer)));
            });
        }
    }
}
