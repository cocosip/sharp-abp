using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Provides methods to access the remote IP address of the current HTTP request.
    /// This implementation considers various HTTP headers commonly used by reverse proxies
    /// and load balancers to determine the original client IP address.
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
        /// This method checks for common HTTP headers used by reverse proxies and load balancers
        /// in the following order:
        /// 1. X-Forwarded-For
        /// 2. X-Real-IP
        /// 3. HttpContext.Connection.RemoteIpAddress
        /// If none of these sources provide a valid IP address, an empty string is returned.
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

                var headers = httpContext.Request.Headers;
                if (headers.TryGetValue("X-Forwarded-For", out StringValues value))
                {
                    var xForwardedFor = value.ToString();
                    var xForwardedForArray = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (xForwardedForArray.Length > 0)
                    {
                        return xForwardedForArray[0];
                    }
                }

                if (headers.TryGetValue("X-Real-IP", out StringValues realIPValue))
                {
                    var xRealIP = realIPValue.ToString();
                    if (!xRealIP.IsNullOrWhiteSpace())
                    {
                        return xRealIP;
                    }
                }

                if (httpContext.Connection.RemoteIpAddress != null)
                {
                    return httpContext.Connection.RemoteIpAddress.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get real remote ip failed. {Message}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the remote IP address from the X-Forwarded-For HTTP header.
        /// The X-Forwarded-For header may contain a comma-separated list of IP addresses,
        /// where the first one is typically the original client IP address.
        /// </summary>
        /// <returns>The remote IP address from X-Forwarded-For header as a string, or an empty string if not found.</returns>
        public virtual string GetXForwardedForRemoteIpAddress()
        {
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext == null)
                {
                    return string.Empty;
                }

                var headers = httpContext.Request.Headers;
                if (headers.TryGetValue("X-Forwarded-For", out StringValues value))
                {
                    var xForwardedFor = value.ToString();
                    var xForwardedForArray = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (xForwardedForArray.Length > 0)
                    {
                        return xForwardedForArray[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get real remote ip failed. {Message}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the remote IP address from the X-Real-IP HTTP header.
        /// This header is commonly used by Nginx and other reverse proxies to pass the original client IP.
        /// </summary>
        /// <returns>The remote IP address from X-Real-IP header as a string, or an empty string if not found.</returns>
        public virtual string GetXRealIPRemoteIpAddress()
        {
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext == null)
                {
                    return string.Empty;
                }

                var headers = httpContext.Request.Headers;

                if (headers.TryGetValue("X-Real-IP", out StringValues value))
                {
                    var xRealIP = value.ToString();
                    if (!xRealIP.IsNullOrWhiteSpace())
                    {
                        return xRealIP;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get real remote ip failed. {Message}", ex.Message);
            }

            return string.Empty;
        }

    }
}