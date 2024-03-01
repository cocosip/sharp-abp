using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace SharpAbp.Abp.TransformSecurityManagement.MongoDB
{
    [DependsOn(
        typeof(AbpTransformSecurityManagementDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class AbpTransformSecurityManagementMongoDbModule : AbpModule
    {
    }
}
