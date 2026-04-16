using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Middleware;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityMiddleware : AbpMiddlewareBase, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityAspNetCoreOptions AspNetCoreOptions { get; }
        protected IServiceProvider ServiceProvider { get; }
        public AbpTransformSecurityMiddleware(
            ILogger<AbpTransformSecurityMiddleware> logger,
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityAspNetCoreOptions> aspNetCoreOptions,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            Options = options.Value;
            AspNetCoreOptions = aspNetCoreOptions.Value;
            ServiceProvider = serviceProvider;
        }

        protected override Task<bool> ShouldSkipAsync(HttpContext context, RequestDelegate next)
        {
            if (Options.IsEnabled)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var identifier = context.Request.Headers[AspNetCoreOptions.TransformSecurityIdentifierName].ToString();
                foreach (var type in AspNetCoreOptions.MiddlewareHandlers)
                {
                    var handler = ServiceProvider.GetRequiredService(type).As<IAbpTransformSecurityMiddlewareHandler>();
                    await handler.HandleAsync(context, identifier, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                return;
            }
            catch (TransformSecurityRequestException ex)
            {
                Logger.LogWarning(ex, "AbpTransformSecurityMiddleware rejected request: {Message}", ex.Message);
                context.Response.StatusCode = ex.StatusCode;
                await context.Response.WriteAsync(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "AbpTransformSecurityMiddleware execute exception: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected fault happened.");
                return;
            }

            await next(context);
        }
    }
}
