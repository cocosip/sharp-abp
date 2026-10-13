using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SharpAbp.Abp.AspNetCore.Http;

public static class SharpAbpForwardedHeadersApplicationBuilderExtensions
{
    private const string RegisteredKey = "SharpAbp.Abp.AspNetCore.Http.ForwardedHeadersRegistered";
    private const string RfcRegisteredKey = "SharpAbp.Abp.AspNetCore.Http.Rfc7239ForwardedHeadersRegistered";

    public static IApplicationBuilder UseSharpAbpForwardedHeaders(this IApplicationBuilder app)
    {
        if (app.Properties.ContainsKey(RegisteredKey))
        {
            return app;
        }

        // Capture the original connection before the native middleware rewrites it.
        app.Use(async (context, next) =>
        {
            context.GetOrCreateForwardedRequestContext(
                app.ApplicationServices.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value);
            await next();
        });
        app.UseForwardedHeaders();
        if (!app.Properties.ContainsKey(RfcRegisteredKey))
        {
            app.UseMiddleware<Rfc7239ForwardedHeadersMiddleware>();
            app.Properties[RfcRegisteredKey] = true;
        }

        app.Properties[RegisteredKey] = true;
        return app;
    }

    public static IApplicationBuilder UseSharpAbpRfc7239ForwardedHeaders(this IApplicationBuilder app)
    {
        if (app.Properties.ContainsKey(RfcRegisteredKey))
        {
            return app;
        }

        app.UseMiddleware<Rfc7239ForwardedHeadersMiddleware>();
        app.Properties[RfcRegisteredKey] = true;
        return app;
    }
}
