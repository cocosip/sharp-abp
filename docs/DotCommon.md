## DotCommon

> DotCommon ABP Adapter, see [`DotCommon`](https://github.com/cocosip/DotCommon)

## Modules

- `SharpAbp.Abp.DotCommon`

## Sample

```c#
[DependsOn(typeof(AbpDotCommonModule))]
public class MyModule : AbpModule {


}

```

- SnowflakeId Generator

```c#
public override void ConfigureServices(ServiceConfigurationContext context){
    Configure<SnowflakeIdOptions>(options=>{
        options.WorkerId = 2;
        options.DatacenterId = 2;
    });
}

public class MyApplicationAppService : ApplicationService 
{
    private readonly ISnowflakeIdGenerator _snowflakeIdGenerator;
    public MyApplicationAppService(ISnowflakeIdGenerator snowflakeIdGenerator)
    {
        _snowflakeIdGenerator = snowflakeIdGenerator;
    }

    public long GenerateId(){
        var id = _snowflakeIdGenerator.Create();
        return id;
    }
}

```