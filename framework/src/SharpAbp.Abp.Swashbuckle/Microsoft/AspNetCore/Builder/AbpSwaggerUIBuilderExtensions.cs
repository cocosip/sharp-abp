using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using Volo.Abp.Swashbuckle;

namespace Microsoft.AspNetCore.Builder
{
    public static class AbpSwaggerUIBuilderExtensions
    {
        public static IApplicationBuilder UseSharpAbpSwaggerUI(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions> setupAction = null)
        {
            var resolver = app.ApplicationServices.GetService<ISwaggerHtmlResolver>();

            return app.UseSwaggerUI(options =>
            {
                options.InjectJavascript("ui/abp.js");
                options.InjectJavascript("ui/abp.swagger.js");
                options.InjectStylesheet("ui/swagger_theme.css");
                options.IndexStream = () => resolver.Resolver();

                setupAction?.Invoke(options);
            });
        }
    }
}
