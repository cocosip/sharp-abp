using SharpAbp.Abp.AspNetCore.TenancyGrouping;

namespace Microsoft.AspNetCore.Builder
{
    public static class AbpAspNetCoreTenancyGroupingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenancyGrouping(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<TenancyGroupingMiddleware>();
        }
    }
}
