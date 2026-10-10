using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    [DependsOn(typeof(SharpAbpOperationRateLimitingModule), typeof(AbpAutofacModule))]
    public class ReplacementTestModule : AbpModule
    {
    }

    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IOperationRateLimitingStore))]
    public class ReplacementAbpStore : CompetitionStore, ISingletonDependency
    {
    }

    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IOperationRateLimitingPolicyProvider))]
    public class ReplacementAbpProvider : ReplacementProvider, ISingletonDependency
    {
    }

    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IOperationRateLimitingFormatter))]
    public class ReplacementAbpFormatter : ReplacementFormatter, ITransientDependency
    {
    }

    public class OperationRateLimitingAbpReplacementTest
    {
        [Fact]
        public async Task Abp_dependency_attributes_replace_store_provider_and_formatter()
        {
            using (var application = AbpApplicationFactory.Create<ReplacementTestModule>(options => options.UseAutofac()))
            {
                application.Initialize();
                Assert.IsType<ReplacementAbpStore>(application.ServiceProvider.GetRequiredService<IOperationRateLimitingStore>());
                Assert.IsType<ReplacementAbpProvider>(application.ServiceProvider.GetRequiredService<IOperationRateLimitingPolicyProvider>());
                Assert.IsType<ReplacementAbpFormatter>(application.ServiceProvider.GetRequiredService<IOperationRateLimitingFormatter>());
                var checker = application.ServiceProvider.GetRequiredService<IOperationRateLimitingChecker>();
                Assert.True(await checker.IsAllowedAsync("race"));
                await checker.CheckAsync("race");
                Assert.False((await checker.GetStatusAsync("race")).IsAllowed);
                var error = await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => checker.CheckAsync("race"));
                Assert.Equal("replacement-duration", error.Data["RetryDescription"]);
                await checker.ResetAsync("race");
                Assert.True(await checker.IsAllowedAsync("race"));
                application.Shutdown();
            }
        }
    }
}
