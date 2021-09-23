using Volo.Abp.AuditLogging;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AuditLogging
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpAuditLoggingDomainModule),
        typeof(AuditLoggingDomainSharedModule)
        )]
    public class AuditLoggingDomainModule : AbpModule
    {

    }
}
