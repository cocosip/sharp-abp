using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore.Http
{
    /// <summary>
    /// Provides extension methods for configuring cookie policy options in the ABP framework.
    /// </summary>
    public static class CookiePolicyOptionsExtensions
    {
        /// <summary>
        /// Adds a SameSite cookie policy to the service configuration context.
        /// This resolves issues with Swagger login failures under non-HTTPS requests.
        /// </summary>
        /// <param name="context">The service configuration context.</param>
        /// <returns>The service configuration context with the cookie policy configured.</returns>
        public static ServiceConfigurationContext AddSameSiteCookiePolicy(this ServiceConfigurationContext context)
        {
            context.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            return context;
        }

        /// <summary>
        /// Checks the SameSite policy for cookies and adjusts it based on the HTTP context and user agent.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="options">The cookie options.</param>
        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (!httpContext.Request.IsHttps || DisallowsSameSiteNone(userAgent))
                {
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        /// <summary>
        /// Determines if the user agent disallows SameSite=None cookies.
        /// </summary>
        /// <param name="userAgent">The user agent string.</param>
        /// <returns>True if the user agent disallows SameSite=None cookies, otherwise false.</returns>
        public static bool DisallowsSameSiteNone(string userAgent)
        {
            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking stack
            if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }
    }
}