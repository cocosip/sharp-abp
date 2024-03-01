using Microsoft.AspNetCore.Builder;

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
            return builder.UseMiddleware<AbpTransformSecurityMiddleware>();
        }

    }
}
