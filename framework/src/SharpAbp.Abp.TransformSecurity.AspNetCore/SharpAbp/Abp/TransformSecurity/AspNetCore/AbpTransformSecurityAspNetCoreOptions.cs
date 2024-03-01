using Volo.Abp.Collections;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityAspNetCoreOptions
    {
        public string TransformSecurityIdName { get; set; } = "AbpTransformSecurityId";

        public ITypeList<IAbpTransformSecurityMiddlewareHandler> MiddlewareHandlers { get; protected set; }

        public AbpTransformSecurityAspNetCoreOptions()
        {
            MiddlewareHandlers = new TypeList<IAbpTransformSecurityMiddlewareHandler>();
        }

    }
}
