using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Defines an interface for accessing HTTP headers in the ABP framework.
    /// </summary>
    public interface IHttpHeaderAccessor
    {
        /// <summary>
        /// Gets the route translation header from the HTTP context.
        /// </summary>
        /// <returns>A RouteTranslationHeader object containing the route translation information.</returns>
        RouteTranslationHeader GetRouteTranslationHeader();

        /// <summary>
        /// Gets headers with a specific prefix from the HTTP context.
        /// </summary>
        /// <param name="prefix">The prefix to filter headers by. If "*", returns all headers.</param>
        /// <returns>A dictionary of header names and values that match the prefix.</returns>
        IDictionary<string, StringValues> GetPrefixHeaders(string prefix = "*");

        /// <summary>
        /// Gets the request host URL from the HTTP context.
        /// </summary>
        /// <returns>The request host URL as a string.</returns>
        string GetRequesHostURL();
    }
}