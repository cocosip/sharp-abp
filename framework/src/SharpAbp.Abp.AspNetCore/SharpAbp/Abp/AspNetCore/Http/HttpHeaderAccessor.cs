using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public class HttpHeaderAccessor : IHttpHeaderAccessor, ITransientDependency
    {
        protected ILogger Logger { get; }
        protected AbpHttpHeadersOptions Options { get; }
        protected IServiceProvider ServiceProvider { get; }
        public HttpHeaderAccessor(
            ILogger<HttpHeaderAccessor> logger,
            IOptions<AbpHttpHeadersOptions> options,
            IServiceProvider serviceProvider)
        {
            Logger = logger;
            Options = options.Value;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// GetRouteTranslationHeader
        /// </summary>
        /// <returns></returns>
        public virtual RouteTranslationHeader GetRouteTranslationHeader()
        {
            var routeTranslationHeader = new RouteTranslationHeader();
            var headers = GetPrefixHeaders(Options.RouteTranslationPrefix);
            var schemeName = FormatHeaderName(Options.RouteTranslationPrefix, "Scheme");
            var hostName = FormatHeaderName(Options.RouteTranslationPrefix, "Host");
            var routerName = FormatHeaderName(Options.RouteTranslationPrefix, "Router");

            foreach (var headerKv in headers)
            {
                if (headerKv.Key == schemeName || headerKv.Key == hostName || headerKv.Key == routerName)
                {
                    if (headerKv.Key == schemeName)
                    {
                        routeTranslationHeader.Scheme = headerKv.Value;
                    }
                    if (headerKv.Key == hostName)
                    {
                        routeTranslationHeader.Host = headerKv.Value;
                    }
                    if (headerKv.Key == routerName)
                    {
                        routeTranslationHeader.Router = headerKv.Value;
                    }
                }
                else
                {
                    routeTranslationHeader.Extends.TryAdd(headerKv.Key, headerKv.Value);
                }
            }

            return routeTranslationHeader;
        }

        /// <summary>
        /// GetPrefixHeader
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public virtual IDictionary<string, StringValues> GetPrefixHeaders(string prefix = "*")
        {
            var headers = new Dictionary<string, StringValues>();
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext != null)
                {
                    if (!prefix.IsNullOrWhiteSpace())
                    {
                        if (prefix.Equals("*"))
                        {
                            return httpContext.Request.Headers;
                        }
                        else
                        {
                            foreach (var headerKv in httpContext.Request.Headers)
                            {
                                if (headerKv.Key.StartsWith(prefix))
                                {
                                    headers[headerKv.Key] = headerKv.Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get http headers failed. {Message}", ex.Message);
            }
            return headers;
        }

        /// <summary>
        /// Get Request HostURL
        /// </summary>
        /// <returns></returns>
        public virtual string GetRequesHostURL()
        {
            var hostURL = "";
            try
            {
                var httpContext = ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

                if (httpContext != null)
                {
                    hostURL = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get http host url failed. {Message}", ex.Message);
            }
            return hostURL;
        }

        protected virtual string FormatHeaderName(string prefix, string name)
        {
            if (prefix.IsNullOrWhiteSpace())
            {
                return name;
            }
            return $"{prefix.TrimEnd('-')}-{name.TrimStart('-')}";
        }
    }
}
