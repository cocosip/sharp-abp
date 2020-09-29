using SharpAbp.Abp.Micro.Discovery.AddressTable;
using SharpAbp.Abp.Micro.Discovery.TestObjects;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Micro.Discovery
{
    [DependsOn(
        typeof(AbpMicroDiscoveryModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AbpMicroDiscoveryTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMicroDiscoveryOptions>(options =>
            {
                options.Discoverers
                    .ConfigureDefault(s => s.UseAddressTableDiscovery(t =>
                    {
                        t.OverrideException = true;
                    }))
                    .Configure<TestServiceDiscoverer1>(s => s.UseAddressTableDiscovery(t =>
                    {
                    }));
            });


            Configure<AddressTableDiscoveryOptions>(options =>
            {
                options.Configure("service1", s =>
                {
                    s.Service = "service1";
                    s.Entries = new List<AddressTableServiceEntry>()
                    {
                        new AddressTableServiceEntry("1","192.168.0.100",10000),
                        new AddressTableServiceEntry("2","192.168.0.101",10001)
                    };
                });
            });

        }
    }
}
