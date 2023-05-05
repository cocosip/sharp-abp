﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AuditLoggingApplicationContractsModule),
        typeof(AuditLoggingDomainModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    public class AuditLoggingApplicationModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AuditLoggingApplicationModule>();
            });

            context.Services.AddAutoMapperObjectMapper<AuditLoggingApplicationModule>();
            return Task.CompletedTask;
        }
    }
}
