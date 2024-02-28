using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity
{
    public class SecurityKeyStore : ISecurityKeyStore, ITransientDependency
    {
        protected IDistributedCache<SecurityKey> Cache { get; }
        public SecurityKeyStore(IDistributedCache<SecurityKey> cache)
        {
            Cache = cache;
        }


        public virtual async Task<SecurityKey> GetAsync(string uniqueId, CancellationToken cancellationToken = default)
        {
            return await Cache.GetAsync(uniqueId, hideErrors: false, token: cancellationToken);
        }

        public virtual async Task SetAsync(SecurityKey securityKey, CancellationToken cancellationToken = default)
        {
            await Cache.SetAsync(securityKey.UniqueId, securityKey, hideErrors: false, token: cancellationToken);
        }


    }
}
