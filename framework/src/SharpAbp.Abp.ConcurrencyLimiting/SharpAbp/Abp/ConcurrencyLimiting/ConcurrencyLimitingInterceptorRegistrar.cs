using System;
using System.Runtime.CompilerServices;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;


namespace SharpAbp.Abp.ConcurrencyLimiting
{
    internal static class ConcurrencyLimitingInterceptorRegistrar
    {
        private sealed class InterceptionDecision
        {
            public bool ShouldIntercept { get; }

            public InterceptionDecision(bool shouldIntercept)
            {
                ShouldIntercept = shouldIntercept;
            }
        }

        private static readonly ConditionalWeakTable<Type, InterceptionDecision> InterceptionDecisions =
            new ConditionalWeakTable<Type, InterceptionDecision>();

        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (InterceptionDecisions.GetValue(context.ImplementationType,
                    type => new InterceptionDecision(ShouldIntercept(type))).ShouldIntercept)
            {
                context.Interceptors.TryAdd<ConcurrencyLimitingInterceptor>();
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            if (DynamicProxyIgnoreTypes.Contains(type) ||
                type.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
            {
                return false;
            }
            return ConcurrencyLimitingInvocationContextResolver.HasPolicy(type) &&
                ConcurrencyLimitingInvocationContextResolver.Warm(type);
        }
    }
}
