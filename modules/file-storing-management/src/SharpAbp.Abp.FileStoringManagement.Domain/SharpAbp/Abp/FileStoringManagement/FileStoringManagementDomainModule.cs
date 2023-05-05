using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring;
using System;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoringManagement
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule),
        typeof(AbpFileStoringAbstractionsModule),
        typeof(AbpAutoMapperModule),
        typeof(FileStoringManagementDomainSharedModule)
        )]
    public class FileStoringManagementDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FileStoringManagementDomainModule>();
            });

            context.Services.AddAutoMapperObjectMapper<FileStoringManagementDomainModule>();

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.CacheConfigurators.Add(cacheName =>
                {
                    if (cacheName == CacheNameAttribute.GetCacheName(typeof(FileStoringContainerCacheItem)))
                    {
                        return new DistributedCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromSeconds(3600)
                        };
                    }
                    return null;
                });
            });

            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.AutoEventSelectors.Add<FileStoringContainer>();
                options.EtoMappings.Add<FileStoringContainer, FileStoringContainerEto>();
            });

            return Task.CompletedTask;
        }

    }
}
