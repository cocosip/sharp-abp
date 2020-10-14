using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
       typeof(AbpFileStoringAliyunModule),
       typeof(AbpFileStoringFastDFSModule),
       typeof(AbpFileStoringAzureModule),
       typeof(AbpFileStoringFileSystemModule),
       typeof(AbpFileStoringMinioModule),
       typeof(AbpFileStoringS3Module),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
    )]
    public class AbpFileStoringAllTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration().GetSection("Containers");

            Configure<AbpFileStoringOptions>(c =>
            {
                c.Configure(configuration);
            });
        }
    }
}
