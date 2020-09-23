using SharpAbp.Abp.CSRedisCore.TestObjects;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.CSRedisCore
{
    [DependsOn(
      typeof(AbpCSRedisCoreModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpCSRedisCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpCSRedisOptions>(options =>
            {
                options.Clients
                    .ConfigureDefault(client =>
                    {
                        client.Mode = RedisMode.Single;
                        client.ConnectionString = "192.168.0.1";
                        client.ReadOnly = true;
                    })
                    .Configure<TestClient1>(client =>
                    {
                        client.Mode = RedisMode.Sentinel;
                        client.ConnectionString = "127.0.0.1";
                        client.ReadOnly = true;
                        client.Sentinels = new List<string>()
                        {
                            "192.168.0.100"
                        };
                    })
                    .Configure<TestClient2>(client =>
                    {
                        client.Mode = RedisMode.Cluster;
                        client.ConnectionString = "127.0.0.2";
                        client.ReadOnly = false;
                    });
            });
        }

    }
}
