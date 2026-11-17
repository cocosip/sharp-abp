using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use AbpTransformSecurity
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAbpTransformSecurity(this IApplicationBuilder builder)
        {
            // Enables the "SecurityKey" rate limiting policy used by the security key endpoint.
            builder.UseRateLimiter();
            return builder.UseMiddleware<AbpTransformSecurityMiddleware>();
        }

    }
}
