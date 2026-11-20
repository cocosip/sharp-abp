using Volo.Abp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Aws;
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
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreSqliteModule),
       typeof(AbpFileStoringAliyunModule),
       typeof(AbpFileStoringAwsModule),
       typeof(AbpFileStoringAzureModule),
       typeof(AbpFileStoringFastDFSModule),
       typeof(AbpFileStoringFileSystemModule),
       typeof(AbpFileStoringMinioModule),
       typeof(AbpFileStoringS3Module),
       typeof(FileStoringManagementApplicationModule),
       typeof(FileStoringManagementEntityFrameworkCoreModule),
       typeof(AbpTestBaseModule),
       typeof(AbpMapperlyModule),
       typeof(AbpAutofacModule)
       )]
    public class FileStoringManagementApplicationTestModule : AbpModule
    {
        private SqliteConnection _connection;

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            _connection.Dispose();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();



            _connection = new SqliteConnection($"Data Source={Guid.NewGuid():N};Mode=Memory;Cache=Shared");
            _connection.Open();
            using (var dbContext = new FileStoringManagementDbContext(new DbContextOptionsBuilder<FileStoringManagementDbContext>().UseSqlite(_connection).Options))
            {
                dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
            }

            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = _connection.ConnectionString;
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.UseSqlite();
                });
            });
        }


    }
}
