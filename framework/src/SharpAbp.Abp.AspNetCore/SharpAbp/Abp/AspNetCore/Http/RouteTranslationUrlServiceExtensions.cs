namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Extension methods for <see cref="IRouteTranslationUrlService"/>.
    /// </summary>
    public static class RouteTranslationUrlServiceExtensions
    {
        /// <summary>
        /// Gets the translation URL using the specified route translation header.
        /// </summary>
        /// <param name="routeTranslationUrlService">The route translation URL service.</param>
        /// <param name="routeTranslationHeader">The route translation header containing scheme, host, router and extends information.</param>
        /// <returns>The translation URL.</returns>
        public static string? GetTranslationUrl(this IRouteTranslationUrlService routeTranslationUrlService, RouteTranslationHeader routeTranslationHeader)
        {
            return routeTranslationUrlService.GetTranslationUrl(routeTranslationHeader.Scheme, routeTranslationHeader.Host, routeTranslationHeader.Router, routeTranslationHeader.Extends);
        }
    }
}