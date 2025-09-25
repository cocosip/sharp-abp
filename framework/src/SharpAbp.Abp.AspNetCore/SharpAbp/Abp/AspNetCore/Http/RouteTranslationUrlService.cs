using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Service for translating and building URLs with route and query parameters
    /// </summary>
    public class RouteTranslationUrlService : IRouteTranslationUrlService, ITransientDependency
    {
        protected ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the RouteTranslationUrlService
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public RouteTranslationUrlService(ILogger<RouteTranslationUrlService> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Builds a complete URL from scheme, host, route and query parameters
        /// </summary>
        /// <param name="scheme">The URL scheme (http, https)</param>
        /// <param name="host">The host name</param>
        /// <param name="router">The route path</param>
        /// <param name="extends">Additional query parameters</param>
        /// <returns>The complete URL or empty string if scheme or host is invalid</returns>
        public virtual string? GetTranslationUrl(string? scheme, string? host, string? router, IDictionary<string, StringValues>? extends)
        {
            try
            {
                // Validate required parameters
                if (scheme.IsNullOrWhiteSpace() || host.IsNullOrWhiteSpace())
                {
                    Logger.LogWarning("Invalid scheme or host provided. Scheme: {Scheme}, Host: {Host}", scheme, host);
                    return string.Empty;
                }

                // Process router path - remove trailing slashes and ensure it starts with /
                var processedRouter = ProcessRouterPath(router);

                // Build the base URL
                var urlBuilder = new StringBuilder($"{scheme}://{host}");

                // Append the processed route path
                if (!processedRouter.IsNullOrWhiteSpace())
                {
                    urlBuilder.Append(processedRouter);
                }

                // Add query parameters if any
                AppendQueryParameters(urlBuilder, extends);

                var result = urlBuilder.ToString();
                Logger.LogDebug("Generated URL: {Url}", result);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while building translation URL");
                return string.Empty;
            }
        }

        /// <summary>
        /// Processes the router path to ensure proper formatting
        /// </summary>
        /// <param name="router">The router path to process</param>
        /// <returns>The processed router path</returns>
        protected virtual string ProcessRouterPath(string? router)
        {
            if (router.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // Remove trailing slashes
            var processed = router!.TrimEnd('/');

            // Replace multiple consecutive slashes with a single slash
            while (processed.Contains("//"))
            {
                processed = processed.Replace("//", "/");
            }

            // Ensure it starts with / if not empty
            if (!processed.IsNullOrWhiteSpace() && !processed.StartsWith("/"))
            {
                processed = "/" + processed;
            }

            return processed;
        }

        /// <summary>
        /// Appends query parameters to the URL builder
        /// </summary>
        /// <param name="urlBuilder">The URL builder to append to</param>
        /// <param name="extends">The query parameters to append</param>
        protected virtual void AppendQueryParameters(StringBuilder urlBuilder, IDictionary<string, StringValues>? extends)
        {
            if (extends == null || extends.Count == 0)
            {
                return;
            }

            var queryParams = extends
                .Where(pair => !pair.Key.IsNullOrEmpty())
                .SelectMany(pair => pair.Value.Where(value => !value.IsNullOrEmpty()),
                           (pair, value) => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(value)}")
                .ToList();

            if (queryParams.Count > 0)
            {
                urlBuilder.Append("?").Append(string.Join("&", queryParams));
            }
        }
    }
}