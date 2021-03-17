using Localization.Resources.AbpUi;
using SharpAbp.Abp.FileStoring.Aliyun;
using SharpAbp.Abp.FileStoring.Azure;
using SharpAbp.Abp.FileStoring.FastDFS;
using SharpAbp.Abp.FileStoring.FileSystem;
using SharpAbp.Abp.FileStoring.Minio;
using SharpAbp.Abp.FileStoring.S3;
using SharpAbp.Abp.FileStoringManagement;
using SharpAbp.Abp.MapTenancyManagement;
using SharpAbp.WebSample.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.TenantManagement;

namespace SharpAbp.WebSample
{
    [DependsOn(
        typeof(WebSampleApplicationContractsModule),
        typeof(AbpAccountHttpApiModule),
        typeof(AbpIdentityHttpApiModule),
        typeof(AbpPermissionManagementHttpApiModule),
        typeof(AbpTenantManagementHttpApiModule),
        typeof(AbpFeatureManagementHttpApiModule),
        typeof(FileStoringManagementHttpApiModule),
        typeof(MapTenancyManagementHttpApiModule),
        typeof(AbpFileStoringAliyunModule),
        typeof(AbpFileStoringFastDFSModule),
        typeof(AbpFileStoringAzureModule),
        typeof(AbpFileStoringFileSystemModule),
        typeof(AbpFileStoringMinioModule),
        typeof(AbpFileStoringS3Module)
        )]
    public class WebSampleHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigureLocalization();
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<WebSampleResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );
            });
        }
    }
}
