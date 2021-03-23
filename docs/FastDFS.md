# FastDFS

> FastDFSCore ABP Adapter, see [`FastDFSCore`](https://github.com/cocosip/FastDFSCore)

## Modules

- `SharpAbp.Abp.FastDFS`
- `SharpAbp.Abp.FastDFS.DotNetty`
- `SharpAbp.Abp.FastDFS.SuperSocket`

## Sample

```c#
[DependsOn(typeof(AbpFastDFSDotNettyModule))]
public class MyModule : AbpModule {


}

public override void ConfigureServices(ServiceConfigurationContext context){
    Configure<FastDFSOption>(options=>{
        var clusterConfiguration = new ClusterConfiguration(){
            Name = "cluster1",
            Trackers = new List<Tracker>()
            {
                new Tracker("127.0.0.1",23000),
            },
            ConnectionTimeout = 10,
            //...
        };
        options.ClusterConfigurations.Add(clusterConfiguration);
    });
}



```