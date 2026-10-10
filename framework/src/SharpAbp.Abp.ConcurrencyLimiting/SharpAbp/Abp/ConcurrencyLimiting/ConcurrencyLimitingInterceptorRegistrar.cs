using System;
using System.Collections.Concurrent;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;


namespace SharpAbp.Abp.ConcurrencyLimiting
{
    internal static class ConcurrencyLimitingInterceptorRegistrar
    {
        private static readonly ConcurrentDictionary<Type, bool> InterceptionDecisions = new ConcurrentDictionary<Type, bool>();

        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (InterceptionDecisions.GetOrAdd(context.ImplementationType, ShouldIntercept))
            {
                context.Interceptors.TryAdd<ConcurrencyLimitingInterceptor>();
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            if (DynamicProxyIgnoreTypes.Contains(type) || type.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
            {
                return false;
            }
            if (type.IsDefined(typeof(ConcurrencyLimitingAttribute), true))
            {
                return ConcurrencyLimitingInvocationContextResolver.Warm(type);
            }
            foreach (var method in type.GetMethods())
            {
                if (method.IsDefined(typeof(ConcurrencyLimitingAttribute), true))
                {
                    return ConcurrencyLimitingInvocationContextResolver.Warm(type);
                }
            }
            return false;
        }
    }
}
