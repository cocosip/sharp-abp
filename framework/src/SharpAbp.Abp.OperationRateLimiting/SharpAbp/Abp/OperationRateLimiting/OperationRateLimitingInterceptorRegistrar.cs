using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public static class OperationRateLimitingInterceptorRegistrar
    {
        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (!DynamicProxyIgnoreTypes.Contains(context.ImplementationType) &&
                OperationRateLimitingInvocationContextResolver.Warm(context.ImplementationType))
            {
                context.Interceptors.TryAdd<OperationRateLimitingInterceptor>();
            }
        }
    }
}
