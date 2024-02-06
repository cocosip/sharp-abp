﻿using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.CryptoVault
{

    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpCryptoVaultDomainSharedModule)
        )]
    public class AbpCryptoVaultDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }


        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            //Configure<AbpAutoMapperOptions>(options =>
            //{
            //    options.AddMaps<DbConnectionsManagementDomainModule>();
            //});

            //context.Services.AddAutoMapperObjectMapper<DbConnectionsManagementDomainModule>();


            //Configure<AbpDistributedCacheOptions>(options =>
            //{
            //    options.CacheConfigurators.Add(cacheName =>
            //    {
            //        if (cacheName == CacheNameAttribute.GetCacheName(typeof(DatabaseConnectionInfoCacheItem)))
            //        {
            //            return new DistributedCacheEntryOptions()
            //            {
            //                SlidingExpiration = TimeSpan.FromSeconds(1800)
            //            };
            //        }
            //        return null;
            //    });
            //});

            //Configure<AbpDistributedEntityEventOptions>(options =>
            //{
            //    options.AutoEventSelectors.Add<DatabaseConnectionInfo>();
            //    options.EtoMappings.Add<DatabaseConnectionInfo, DatabaseConnectionInfoEto>();
            //});
            return Task.CompletedTask;
        }

    }
}
