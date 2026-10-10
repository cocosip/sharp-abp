using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingInterceptor : AbpInterceptor, ITransientDependency
    {
        protected IOperationRateLimitingChecker Checker { get; }

        public OperationRateLimitingInterceptor(IOperationRateLimitingChecker checker)
        {
            Checker = checker;
        }

        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (invocation.TargetObject == null)
            {
                throw new OperationRateLimitingConfigurationException("Operation limiting requires an invocation target.");
            }
            var metadata = OperationRateLimitingInvocationContextResolver.GetMetadata(invocation.TargetObject.GetType(), invocation.Method);
            if (metadata.PolicyName == null)
            {
                await invocation.ProceedAsync().ConfigureAwait(false);
                return;
            }
            if (metadata.Error != null)
            {
                throw new OperationRateLimitingConfigurationException(metadata.Error);
            }
            var token = CancellationToken.None;
            if (metadata.TokenIndex >= 0)
            {
                if (!(invocation.Arguments[metadata.TokenIndex] is CancellationToken suppliedToken))
                {
                    throw new OperationRateLimitingConfigurationException("Invalid CancellationToken argument.");
                }
                token = suppliedToken;
            }
            await Checker.CheckAsync(metadata.PolicyName,
                OperationRateLimitingInvocationContextResolver.ResolveContext(invocation, metadata), token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            await invocation.ProceedAsync().ConfigureAwait(false);
        }
    }
}
