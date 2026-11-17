using Volo.Abp.Collections;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityAspNetCoreOptions
    {
        public string TransformSecurityIdentifierName { get; set; } = "AbpTransformSecurityIdentifier";

        /// <summary>
        /// Gets or sets how many security key requests a single client can make per window.
        /// </summary>
        public int SecurityKeyRateLimitPermitLimit { get; set; } = 60;

        /// <summary>
        /// Gets or sets the security key rate limiting window in seconds.
        /// </summary>
        public int SecurityKeyRateLimitWindowSeconds { get; set; } = 60;

        public ITypeList<IAbpTransformSecurityMiddlewareHandler> MiddlewareHandlers { get; protected set; }

        public AbpTransformSecurityAspNetCoreOptions()
        {
            MiddlewareHandlers = new TypeList<IAbpTransformSecurityMiddlewareHandler>();
        }

    }
}
