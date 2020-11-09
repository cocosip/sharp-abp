using SharpAbp.Abp.FreeRedis.TestObjects;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.FreeRedis
{
    [DependsOn(
      typeof(AbpFreeRedisModule),
      typeof(AbpTestBaseModule),
      typeof(AbpAutofacModule)
      )]
    public class AbpFreeRedisTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFreeRedisOptions>(options =>
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
