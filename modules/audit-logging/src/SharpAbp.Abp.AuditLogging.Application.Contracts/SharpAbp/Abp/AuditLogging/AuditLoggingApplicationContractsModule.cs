using Volo.Abp.Application;
using Volo.Abp.AuditLogging;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AbpAuditLoggingDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class AuditLoggingApplicationContractsModule : AbpModule
    {

    }
}
