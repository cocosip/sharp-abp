using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    [DependsOn(typeof(SharpAbpConcurrencyLimitingModule), typeof(AbpTestBaseModule), typeof(AbpAutofacModule))]
    public class SharpAbpConcurrencyLimitingTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<SharpAbpConcurrencyLimitingOptions>(options => options
                .AddPolicy("class", b => b.WithConcurrency(1))
                .AddPolicy("method", b => b.WithConcurrency(1))
                .AddPolicy("parameter", b => b.WithConcurrency(1).PartitionByParameter())
                .AddPolicy("interface", b => b.WithConcurrency(1)));
            context.Services.AddTransient<ProxyOperations>();
            context.Services.AddTransient<InvalidOperations>();
            context.Services.AddTransient<IInterfaceOperations, InterfaceOperations>();
            context.Services.AddTransient<IInterfaceAttributedOperations, InterfaceAttributedOperations>();
        }
    }

}
