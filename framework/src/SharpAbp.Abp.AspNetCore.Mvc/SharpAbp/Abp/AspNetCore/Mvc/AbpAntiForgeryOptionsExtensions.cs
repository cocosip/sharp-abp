using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.AspNetCore.Mvc
{
    public static class AbpAntiForgeryOptionsExtensions
    {

        /// <summary>
        /// Other AntiForgery Options
        /// options.TokenCookie.Expiration = TimeSpan.FromDays(365);
        /// options.AutoValidateIgnoredHttpMethods.Add("POST");
        /// options.TokenCookie.Expiration = TimeSpan.Zero;
        /// options.AutoValidate = false; //Do not validate antiforgery token
        /// options.AutoValidateIgnoredHttpMethods.Remove("GET");
        /// options.AutoValidateFilter = type => !type.Namespace.StartsWith("MyProject.MyIgnoredNamespace");
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ServiceConfigurationContext AddDisableAntiForgery(this ServiceConfigurationContext context)
        {
            context.Services.Configure<AbpAntiForgeryOptions>(options =>
            {
                options.TokenCookie.Expiration = TimeSpan.Zero;
                //Disable antiforgery token validation
                options.AutoValidate = false;
            });

            return context;
        }
    }
}
