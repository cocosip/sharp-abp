﻿using SharpAbp.Abp.FileStoring.FileSystem.Localization;
using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    [DependsOn(
        typeof(AbpFileStoringModule)
        )]
    public class AbpFileStoringFileSystemModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<AbpFileStoringAbstractionsOptions>(c =>
            {
                var configuration = GetFileProviderConfiguration();
                c.Providers.TryAdd(configuration);
            });
            return Task.CompletedTask;
        }


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpFileStoringFileSystemModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FileStoringFileSystemResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/SharpAbp/Abp/FileStoring/FileSystem/Localization/Resources");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FileStoringFastDFS", typeof(FileStoringFileSystemResource));
            });

            return Task.CompletedTask;
        }


        private FileProviderConfiguration GetFileProviderConfiguration()
        {
            var configuration = new FileProviderConfiguration(
                FileSystemFileProviderConfigurationNames.ProviderName,
                typeof(FileStoringFileSystemResource));

            configuration.DefaultNamingNormalizers.TryAdd<FileSystemFileNamingNormalizer>();
            configuration
                .AddItem(FileSystemFileProviderConfigurationNames.BasePath, typeof(string), "/data1")
                .AddItem(FileSystemFileProviderConfigurationNames.AppendContainerNameToBasePath, typeof(bool), "false")
                .AddItem(FileSystemFileProviderConfigurationNames.HttpServer, typeof(string), "http://192.168.0.100:8080");
            return configuration;
        }
    }
}
