using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using SharpAbp.Abp.FileStoringManagement.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
       typeof(FileStoringManagementApplicationModule),
       typeof(FileStoringManagementEntityFrameworkCoreModule),
       typeof(AbpFileStoringAliyunModule),
       typeof(AbpFileStoringAzureModule),
       typeof(AbpFileStoringFastDFSModule),
       typeof(AbpFileStoringFileSystemModule),
       typeof(AbpFileStoringMinioModule),
       typeof(AbpFileStoringS3Module),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
       )]
    public class FileStoringManagementApplicationTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();

            context.Services.AddEntityFrameworkInMemoryDatabase();

            var databaseName = Guid.NewGuid().ToString();

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.DbContextOptions.UseInMemoryDatabase(databaseName);
                });
            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled; //EF in-memory database does not support transactions
            });


        }
    }
}
