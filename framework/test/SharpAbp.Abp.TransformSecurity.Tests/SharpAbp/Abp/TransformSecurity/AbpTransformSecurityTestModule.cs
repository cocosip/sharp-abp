using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurity
{
    /// <summary>
    /// Test module for TransformSecurity functionality
    /// </summary>
    [DependsOn(
        typeof(AbpTransformSecurityModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
    )]
    public class AbpTransformSecurityTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            // Configure test-specific options
            Configure<AbpTransformSecurityOptions>(options =>
            {
                options.IsEnabled = true;
                options.EncryptionAlgo = "RSA";
                options.Expires = TimeSpan.FromMinutes(10);
                options.BizTypes.Add("Login");
                options.BizTypes.Add("UpdatePassword");
                options.BizTypes.Add("TestBizType");
            });

            return Task.CompletedTask;
        }
    }
}
