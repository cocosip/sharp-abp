using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.MassTransit
{
    /// <summary>
    /// Just library ref
    /// </summary>
    public class AbpMassTransitModule : AbpModule
    {

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            var busControl = context.ServiceProvider.GetService<IBusControl>();
            busControl.Stop(TimeSpan.FromSeconds(10));
        }
    }
}
