using System.Threading.Tasks;
using Volo.Abp.DynamicProxy;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public class ConcurrencyLimitingInterceptor : AbpInterceptor, ITransientDependency
    {
        protected IConcurrencyLimitingManager Manager { get; }
        public ConcurrencyLimitingInterceptor(IConcurrencyLimitingManager manager)
        {
            Manager = manager;
        }

        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            var resolved = ConcurrencyLimitingInvocationContextResolver.Resolve(invocation);
            if (resolved.PolicyName == null)
            {
                await invocation.ProceedAsync().ConfigureAwait(false);
                return;
            }
            var token = resolved.CancellationToken;
            using (await Manager.AcquireAsync(resolved.PolicyName, resolved.Context, token).ConfigureAwait(false))
            {
                token.ThrowIfCancellationRequested();
                await invocation.ProceedAsync().ConfigureAwait(false);
            }
        }
    }

}
