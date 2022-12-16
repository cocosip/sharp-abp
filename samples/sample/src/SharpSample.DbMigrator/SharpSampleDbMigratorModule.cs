using SharpSample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpSample.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SharpSampleEntityFrameworkCoreModule),
    typeof(SharpSampleApplicationContractsModule)
    )]
public class SharpSampleDbMigratorModule : AbpModule
{

}
