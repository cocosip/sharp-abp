using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpAbp.Abp.FileStoring;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FileStoringSample
{

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpFileStoringAliyunModule),
        typeof(AbpFileStoringFastDFSModule),
        typeof(AbpFileStoringAzureModule),
        typeof(AbpFileStoringFileSystemModule),
        typeof(AbpFileStoringMinioModule),
        typeof(AbpFileStoringS3Module),
        typeof(AbpAutofacModule)
        )]
    public class FileStoringSampleModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            context.Services.AddHostedService<FileStoringSampleHostedService>();

            //Section
            var section = context.Services.GetConfiguration().GetSection("Containers");
            Configure<AbpFileStoringOptions>(c =>
            {
                c.Configure(section);
            });

        }
    }
}
