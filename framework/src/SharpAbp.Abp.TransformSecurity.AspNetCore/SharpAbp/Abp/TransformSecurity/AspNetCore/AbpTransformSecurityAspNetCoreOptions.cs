using Volo.Abp.Collections;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityAspNetCoreOptions
    {
        public string TransformSecurityIdentifierName { get; set; } = "AbpTransformSecurityIdentifier";

        public ITypeList<IAbpTransformSecurityMiddlewareHandler> MiddlewareHandlers { get; protected set; }

        public AbpTransformSecurityAspNetCoreOptions()
        {
            MiddlewareHandlers = new TypeList<IAbpTransformSecurityMiddlewareHandler>();
        }

    }
}
