using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityCredentialStore : ISecurityCredentialStore, ITransientDependency
    {
        protected IDistributedCache<SecurityCredential> Cache { get; }
        public SecurityCredentialStore(IDistributedCache<SecurityCredential> cache)
        {
            Cache = cache;
        }

        public virtual async Task<SecurityCredential?> GetAsync(string identifier, CancellationToken cancellationToken = default)
        {
            return await Cache.GetAsync(identifier, token: cancellationToken);
        }

        public virtual async Task SetAsync(SecurityCredential credential, CancellationToken cancellationToken = default)
        {
            await Cache.SetAsync(credential.Identifier!, credential, token: cancellationToken);
        }
    }
}
