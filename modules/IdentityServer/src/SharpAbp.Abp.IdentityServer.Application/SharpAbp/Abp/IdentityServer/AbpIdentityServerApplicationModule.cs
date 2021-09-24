﻿using SharpAbp.Abp.Identity;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.IdentityServer
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(IdentityServerApplicationContractsModule),
        typeof(IdentityServerDomainModule),
        typeof(IdentityApplicationModule)
        )]
    public class AbpIdentityServerApplicationModule : AbpModule
    {

    }
}