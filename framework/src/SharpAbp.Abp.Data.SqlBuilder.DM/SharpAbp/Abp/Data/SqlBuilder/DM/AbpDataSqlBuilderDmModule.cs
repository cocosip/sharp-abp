using SharpAbp.Abp.EntityFrameworkCore.DM;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Data.SqlBuilder.DM
{
    [DependsOn(
        typeof(AbpDataSqlBuilderModule),
        typeof(AbpEntityFrameworkCoreDmModule)
    )]
    public class AbpDataSqlBuilderDmModule : AbpModule
    {

    }
}