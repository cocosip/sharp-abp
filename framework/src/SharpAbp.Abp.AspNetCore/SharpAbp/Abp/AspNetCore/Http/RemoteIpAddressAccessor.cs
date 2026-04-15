using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Provides methods to access the remote IP address of the current HTTP request.
    /// This implementation reads the normalized remote IP address from the current connection.
    /// Reverse-proxy scenarios should be handled by ASP.NET Core forwarded header middleware.
    /// </summary>
    public class RemoteIpAddressAccessor : IRemoteIpAddressAccessor, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteIpAddressAccessor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public RemoteIpAddressAccessor(
            ILogger<RemoteIpAddressAccessor> logger,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the remote IP address of the current HTTP request.
        /// This method reads the normalized connection address. If forwarded headers are used,
        /// they should be validated and applied by ASP.NET Core before this accessor is called.
        /// </summary>
        /// <returns>The remote IP address as a string, or an empty string if not found.</returns>
        public virtual string GetRemoteIpAddress()
        {
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext == null)
                {
                    return string.Empty;
                }

                return httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get remote IP address from HTTP context.");
            }

            return string.Empty;
        }

        /// <summary>
        /// Raw proxy headers are not trusted here. Configure forwarded header middleware and use
        /// <see cref="GetRemoteIpAddress"/> to read the normalized connection address instead.
        /// </summary>
        /// <returns>An empty string.</returns>
        public virtual string GetXForwardedForRemoteIpAddress()
        {
            return string.Empty;
        }

        /// <summary>
        /// Raw proxy headers are not trusted here. Configure forwarded header middleware and use
        /// <see cref="GetRemoteIpAddress"/> to read the normalized connection address instead.
        /// </summary>
        /// <returns>An empty string.</returns>
        public virtual string GetXRealIPRemoteIpAddress()
        {
            return string.Empty;
        }

    }
}
