using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.Crypto.RSA;
using SharpAbp.Abp.Crypto.SM2;
using SharpAbp.Abp.TransformSecurity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ISecurityCredentialManager))]
    public class DatabaseSecurityCredentialManager : ISecurityCredentialManager, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityRSAOptions RSAOptions { get; }
        protected AbpTransformSecuritySM2Options SM2Options { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected IClock Clock { get; }
        protected ISecurityCredentialStore SecurityCredentialStore { get; }

        public DatabaseSecurityCredentialManager()
        {
                
        }

        public virtual async Task<SecurityCredential> GenerateAsync(string bizType, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected virtual bool ValidateBizType(string bizType)
        {
            foreach (var item in Options.BizTypes)
            {
                if (item.Equals(bizType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
