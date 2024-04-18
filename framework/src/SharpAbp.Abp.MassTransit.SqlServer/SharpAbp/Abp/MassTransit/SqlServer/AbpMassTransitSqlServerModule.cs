using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit.SqlServer
{
    [DependsOn(
        typeof(AbpMassTransitModule)
        )]
    public class AbpMassTransitSqlServerModule : AbpModule
    {


    }
}
