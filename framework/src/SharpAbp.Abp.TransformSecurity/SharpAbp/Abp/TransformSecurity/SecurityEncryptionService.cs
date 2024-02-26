using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityEncryptionService : ISecurityEncryptionService, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; set; }
    }
}
