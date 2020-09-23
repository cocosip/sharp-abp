using SharpAbp.Abp.Consul.TestObjects;
using System;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Consul
{
    [DependsOn(
      typeof(AbpConsulModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpConsulTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpConsulOptions>(options =>
            {
                options.Consuls
                    .ConfigureDefault(consul =>
                    {
                        consul.Address = new Uri("http://127.0.0.1");
                        consul.DataCenter = "DataCenter1";
                        consul.Token = "123";
                        consul.WaitTime = TimeSpan.FromSeconds(5);
                    })
                    .Configure<TestConsul2>(consul =>
                    {
                        consul.Address = new Uri("http://192.168.0.100/api");
                        consul.DataCenter = "DataCenter2";
                        consul.Token = "456";
                        consul.WaitTime = null;
                    })
                    .Configure<TestConsul3>(consul =>
                    {
                        consul.Address = new Uri("http://192.168.0.103");
                        consul.DataCenter = "DataCenter3";
                        consul.Token = "111";
                        consul.WaitTime = TimeSpan.FromSeconds(3);
                        consul.ClientOverride = c => { };
                        consul.HandlerOverride = c => { };
                    });
            });
        }
    }
}
