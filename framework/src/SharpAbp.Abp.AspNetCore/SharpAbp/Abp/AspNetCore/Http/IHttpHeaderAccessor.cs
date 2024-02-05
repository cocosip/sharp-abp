using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace SharpAbp.Abp.AspNetCore.Http
{
    public interface IHttpHeaderAccessor
    {
        /// <summary>
        /// GetRouteTranslationHeader
        /// </summary>
        /// <returns></returns>
        RouteTranslationHeader GetRouteTranslationHeader();

        /// <summary>
        /// GetPrefixHeader
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        IDictionary<string, StringValues> GetPrefixHeaders(string prefix = "*");

        /// <summary>
        /// Get Request HostURL
        /// </summary>
        /// <returns></returns>
        string GetRequesHostURL();
    }
}
