using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class RemoteIpAddressAccessor : IRemoteIpAddressAccessor, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        public RemoteIpAddressAccessor(
            ILogger<RemoteIpAddressAccessor> logger,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Get remote ip address
        /// </summary>
        /// <returns></returns>
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
                if (headers.ContainsKey("X-Forwarded-For"))
                {
                    var xForwardedFor = headers["X-Forwarded-For"].ToString();
                    var xForwardedForArray = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (xForwardedForArray.Length > 0)
                    {
                        return xForwardedForArray[0];
                    }
                }

                if (headers.ContainsKey("X-Real-IP"))
                {
                    var xRealIP = headers["X-Real-IP"].ToString();
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
                Logger.LogError(ex, "Get real remote ip failed.{0}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get remote ip address from X-Forwarded-For
        /// </summary>
        /// <returns></returns>
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
                if (headers.ContainsKey("X-Forwarded-For"))
                {
                    var xForwardedFor = headers["X-Forwarded-For"].ToString();
                    var xForwardedForArray = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (xForwardedForArray.Length > 0)
                    {
                        return xForwardedForArray[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get real remote ip failed.{0}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get remote ip address from X-Real-IP
        /// </summary>
        /// <returns></returns>
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

                if (headers.ContainsKey("X-Real-IP"))
                {
                    var xRealIP = headers["X-Real-IP"].ToString();
                    if (!xRealIP.IsNullOrWhiteSpace())
                    {
                        return xRealIP;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get real remote ip failed.{0}", ex.Message);
            }

            return string.Empty;
        }

    }
}