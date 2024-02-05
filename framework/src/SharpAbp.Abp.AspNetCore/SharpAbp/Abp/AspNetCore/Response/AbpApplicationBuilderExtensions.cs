using Microsoft.AspNetCore.Builder;

namespace SharpAbp.Abp.AspNetCore.Response
{
    public static class AbpApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseResponseHeader(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseHeaderMiddleware>();
        }
    }
}
