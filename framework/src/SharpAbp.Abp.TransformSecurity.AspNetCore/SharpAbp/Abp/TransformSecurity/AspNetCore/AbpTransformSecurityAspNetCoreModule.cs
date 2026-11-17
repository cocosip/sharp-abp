using System;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    [DependsOn(
        typeof(AbpTransformSecurityModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class AbpTransformSecurityAspNetCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpTransformSecurityAspNetCoreOptions>(options =>
            {
                options.MiddlewareHandlers.Add<PasswordGrantTokenHandler>();
            });

            context.Services.AddRateLimiter(_ => { });
            context.Services
                .AddOptions<RateLimiterOptions>()
                .Configure<Microsoft.Extensions.Options.IOptions<AbpTransformSecurityAspNetCoreOptions>>((rateLimiterOptions, aspNetCoreOptions) =>
                {
                    var securityKeyOptions = aspNetCoreOptions.Value;
                    rateLimiterOptions.AddPolicy(
                        TransformSecurityRatelimitNames.SecurityKeyRateLimiting,
                        httpContext => RateLimitPartition.GetFixedWindowLimiter(
                            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = securityKeyOptions.SecurityKeyRateLimitPermitLimit,
                                Window = TimeSpan.FromSeconds(securityKeyOptions.SecurityKeyRateLimitWindowSeconds),
                                QueueLimit = 0
                            }));
                });

            return Task.CompletedTask;
        }
    }
}
