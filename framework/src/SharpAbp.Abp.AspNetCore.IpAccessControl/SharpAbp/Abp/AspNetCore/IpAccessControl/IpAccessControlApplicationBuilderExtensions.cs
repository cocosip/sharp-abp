using Microsoft.AspNetCore.Builder;

namespace SharpAbp.Abp.AspNetCore.IpAccessControl
{
    public static class IpAccessControlApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIpAccessControl(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IpAccessControlMiddleware>();
        }
    }
}
