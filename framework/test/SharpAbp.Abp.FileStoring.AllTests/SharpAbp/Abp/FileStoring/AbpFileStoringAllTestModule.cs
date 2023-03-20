using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Aws;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.KS3;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.Obs;
using SharpAbp.Abp.FileStoring.S3;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
       typeof(AbpFileStoringAliyunModule),
       typeof(AbpFileStoringObsModule),
       typeof(AbpFileStoringFastDFSModule),
       typeof(AbpFileStoringAzureModule),
       typeof(AbpFileStoringAwsModule),
       typeof(AbpFileStoringFileSystemModule),
       typeof(AbpFileStoringMinioModule),
       typeof(AbpFileStoringKS3Module),
       typeof(AbpFileStoringS3Module),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
    )]
    public class AbpFileStoringAllTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpFileStoringOptions>(c =>
            {
                c.Configure(configuration, context);
            });
        }


    }
}
