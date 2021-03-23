# AutoS3

> Base on `AWSSDK.S3`, manager `IAmazon` client, and reused. See [`AutoS3`](https://github.com/cocosip/AutoS3) source project  

## Modules

- `SharpAbp.Abp.AutoS3`
- `SharpAbp.Abp.AutoS3.KS3`

## Sample

``` c#
//Module DependsOn
[DependsOn(typeof(AbpAutoS3Module))]
public class MyModule : AbpModule {


}

//Configure
public override void ConfigureServices(ServiceConfigurationContext context){
    Configure<AutoS3Options>(options =>
    {
        options.Clients.Configure("default",c=>{
            c.Vendor = S3VendorType.Amazon;
            c.AccessKeyId = "";
            c.SecretAccessKey = "";
            c.Config = new AmazonS3Config(){
                //...
            };
        });
    });
}

```