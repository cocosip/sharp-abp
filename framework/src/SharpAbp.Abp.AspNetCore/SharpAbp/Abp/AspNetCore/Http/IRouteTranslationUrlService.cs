using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public interface IRouteTranslationUrlService
    {
        /// <summary>
        /// Builds a complete URL from scheme, host, route and query parameters
        /// </summary>
        /// <param name="scheme">The URL scheme (http, https)</param>
        /// <param name="host">The host name</param>
        /// <param name="router">The route path</param>
        /// <param name="extends">Additional query parameters</param>
        /// <returns>The complete URL or empty string if scheme or host is invalid</returns>
        string? GetTranslationUrl(string? scheme, string? host, string? router, IDictionary<string, StringValues> extends);
    }
}