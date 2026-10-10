using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    internal static class TestServices
    {
        public static ServiceProvider Create(SharpAbpConcurrencyLimitingOptions options)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IOptions<SharpAbpConcurrencyLimitingOptions>>(Options.Create(options));
            services.AddSingleton<ConcurrencyLimitingConfiguration>();
            services.AddSingleton<IConcurrencyLimitingConfiguration>(sp => sp.GetRequiredService<ConcurrencyLimitingConfiguration>());
            services.AddSingleton<IConcurrencyLimitingPolicyProvider, OptionsConcurrencyLimitingPolicyProvider>();
            services.AddSingleton<ConcurrencyLimiterRegistry>();
            services.AddSingleton<IConcurrencyLimiterRegistry>(sp => sp.GetRequiredService<ConcurrencyLimiterRegistry>());
            services.AddSingleton<ICurrentTenantAccessor>(AsyncLocalCurrentTenantAccessor.Instance);
            services.AddTransient<ICurrentTenant, CurrentTenant>();
            services.AddSingleton<ICurrentPrincipalAccessor, ThreadCurrentPrincipalAccessor>();
            services.AddTransient<ICurrentUser, CurrentUser>();
            services.AddScoped<ScopeIdentity>();
            services.AddScoped<ConcurrencyLimitingPartitionResolver>();
            services.AddScoped<IConcurrencyLimitingPartitionResolver>(sp => sp.GetRequiredService<ConcurrencyLimitingPartitionResolver>());
            services.AddTransient<IConcurrencyLimitingManager, ConcurrencyLimitingManager>();
            var result = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = false });
            try
            {
                result.GetRequiredService<IConcurrencyLimitingPolicyProvider>();
            }
            catch { result.Dispose(); throw; }
            return result;
        }
    }
    internal class ScopeIdentity
    {
        public string Id { get; } = Guid.NewGuid().ToString();
    }

}
