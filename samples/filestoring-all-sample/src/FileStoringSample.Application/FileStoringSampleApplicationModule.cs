using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using SharpAbp.Abp.FileStoringManagement;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace FileStoringSample
{
    [DependsOn(
        typeof(FileStoringSampleDomainModule),
        typeof(AbpFileStoringAliyunModule),
        typeof(AbpFileStoringAzureModule),
        typeof(AbpFileStoringFastDFSModule),
        typeof(AbpFileStoringFileSystemModule),
        typeof(AbpFileStoringMinioModule),
        typeof(AbpFileStoringS3Module),
        typeof(FileStoringManagementApplicationModule),
        typeof(AbpAccountApplicationModule),
        typeof(FileStoringSampleApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
        )]
    public class FileStoringSampleApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FileStoringSampleApplicationModule>();
            });
        }
    }
}
