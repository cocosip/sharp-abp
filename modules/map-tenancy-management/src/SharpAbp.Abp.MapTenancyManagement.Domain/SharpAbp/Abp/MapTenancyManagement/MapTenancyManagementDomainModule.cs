﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(AbpCachingModule),
        typeof(AbpMapTenancyModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpTenantManagementDomainModule),
        typeof(MapTenancyManagementDomainSharedModule)
        )]
    public class MapTenancyManagementDomainModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MapTenancyManagementDomainModule>();
            });

            context.Services.AddAutoMapperObjectMapper<MapTenancyManagementDomainModule>();

            // Register IMapTenantManager interface with MapTenantManager implementation
            context.Services.AddTransient<IMapTenantManager, MapTenantManager>();

            Configure<MapTenancyStoreOptions>(options => { });

            Configure<AbpDistributedEntityEventOptions>(options =>
            {
                options.AutoEventSelectors.Add<MapTenant>();
                options.EtoMappings.Add<MapTenant, MapTenantEto>();
            });
            return Task.CompletedTask;
        }
    }
}
