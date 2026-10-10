using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Volo.Abp.DynamicProxy;


namespace SharpAbp.Abp.ConcurrencyLimiting
{
    internal static class ConcurrencyLimitingInvocationContextResolver
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, InvocationMetadata>> MetadataCache =
            new ConcurrentDictionary<Type, ConcurrentDictionary<MethodInfo, InvocationMetadata>>();

        private sealed class InvocationMetadata
        {
            public string? PolicyName { get; }
            public int ParameterIndex { get; }
            public int TokenIndex { get; }
            public string? ConfigurationError { get; }

            public InvocationMetadata(string? policyName, int parameterIndex, int tokenIndex, string? configurationError)
            {
                PolicyName = policyName;
                ParameterIndex = parameterIndex;
                TokenIndex = tokenIndex;
                ConfigurationError = configurationError;
            }
        }

        public static bool Warm(Type implementationType)
        {
            var methods = MetadataCache.GetOrAdd(implementationType,
                _ => new ConcurrentDictionary<MethodInfo, InvocationMetadata>());
            var hasPolicy = false;
            var classHasPolicy = implementationType.IsDefined(typeof(ConcurrencyLimitingAttribute), true);
            foreach (var method in implementationType.GetMethods())
            {
                if (method.DeclaringType == typeof(object) ||
                    (!classHasPolicy && !method.IsDefined(typeof(ConcurrencyLimitingAttribute), true)))
                {
                    continue;
                }
                var metadata = methods.GetOrAdd(method, candidate => BuildMetadata(implementationType, candidate));
                hasPolicy = hasPolicy || metadata.PolicyName != null;
            }
            return hasPolicy;
        }

        private static InvocationMetadata GetMetadata(IAbpMethodInvocation invocation)
        {
            // ABP provides the actual implementation MethodInfo; it invokes business code itself.
            var target = invocation.TargetObject;
            if (target == null)
            {
                throw new ConcurrencyLimitingConfigurationException("ConcurrencyLimiting requires an instance service invocation.");
            }
            var type = target.GetType();
            if (MetadataCache.TryGetValue(type, out var cachedMethods) &&
                cachedMethods.TryGetValue(invocation.Method, out var cachedMetadata))
            {
                return cachedMetadata;
            }
            return GetMetadataOnCacheMiss(type, invocation.Method);
        }

        private static InvocationMetadata GetMetadataOnCacheMiss(Type actualType, MethodInfo method)
        {
            var type = actualType;
            while (!MetadataCache.ContainsKey(type) && type.BaseType != null)
            {
                type = type.BaseType;
            }
            var methods = MetadataCache.GetOrAdd(type, _ => new ConcurrentDictionary<MethodInfo, InvocationMetadata>());
            MetadataCache.TryAdd(actualType, methods);
            return methods.GetOrAdd(method, candidate => BuildMetadata(type, candidate));
        }

        private static InvocationMetadata BuildMetadata(Type type, MethodInfo method)
        {
            if (method.DeclaringType == typeof(object) || method.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true) ||
                type.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
            {
                return new InvocationMetadata(null, -1, -1, null);
            }

            var policy = method.GetCustomAttribute<ConcurrencyLimitingAttribute>(true)?.PolicyName ??
                type.GetCustomAttribute<ConcurrencyLimitingAttribute>(true)?.PolicyName;
            if (policy == null)
            {
                return new InvocationMetadata(null, -1, -1, null);
            }
            var parameterIndex = -1;
            var tokenIndex = -1;
            string? error = null;
            var returnType = method.ReturnType;
            if (returnType != typeof(System.Threading.Tasks.Task) &&
                (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(System.Threading.Tasks.Task<>)))
            {
                error = "ConcurrencyLimiting supports only Task and Task<T> methods.";
            }
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(CancellationToken))
                {
                    if (tokenIndex >= 0)
                    {
                        error = "Multiple CancellationToken arguments are not supported.";
                    }
                    tokenIndex = i;
                }
                if (parameters[i].IsDefined(typeof(ConcurrencyLimitingParameterAttribute), true))
                {
                    if (parameterIndex >= 0)
                    {
                        error = "Multiple concurrency parameter markers are not supported.";
                    }
                    if (parameters[i].ParameterType != typeof(string))
                    {
                        error = "A marked concurrency parameter must be a string.";
                    }
                    parameterIndex = i;
                }
            }
            return new InvocationMetadata(policy, parameterIndex, tokenIndex, error);
        }

        internal sealed class ResolvedInvocation
        {
            public string? PolicyName { get; }
            public CancellationToken CancellationToken { get; }
            public ConcurrencyLimitingContext Context { get; }

            public ResolvedInvocation(string? policyName, CancellationToken cancellationToken, ConcurrencyLimitingContext context)
            {
                PolicyName = policyName;
                CancellationToken = cancellationToken;
                Context = context;
            }
        }

        public static ResolvedInvocation Resolve(IAbpMethodInvocation invocation)
        {
            var metadata = GetMetadata(invocation);
            if (metadata.PolicyName == null)
            {
                return new ResolvedInvocation(null, CancellationToken.None, new ConcurrencyLimitingContext());
            }
            if (metadata.ConfigurationError != null)
            {
                throw new ConcurrencyLimitingConfigurationException(metadata.ConfigurationError);
            }
            var token = CancellationToken.None;
            if (metadata.TokenIndex >= 0)
            {
                if (!(invocation.Arguments[metadata.TokenIndex] is CancellationToken suppliedToken))
                {
                    throw new ConcurrencyLimitingConfigurationException("The cancellation argument must be a CancellationToken value.");
                }
                token = suppliedToken;
            }
            if (metadata.ParameterIndex >= 0)
            {
                return new ResolvedInvocation(metadata.PolicyName, token,
                    new ConcurrencyLimitingContext { Parameter = (string?)invocation.Arguments[metadata.ParameterIndex] });
            }

            IHasConcurrencyLimitingParameter? candidate = null;
            foreach (var argument in invocation.Arguments)
            {
                if (argument is IHasConcurrencyLimitingParameter dto)
                {
                    if (candidate != null)
                    {
                        throw new ConcurrencyLimitingConfigurationException("Multiple concurrency parameter DTOs are not supported.");
                    }
                    candidate = dto;
                }
            }
            return new ResolvedInvocation(metadata.PolicyName, token,
                new ConcurrencyLimitingContext { Parameter = candidate?.GetPartitionParameter() });
        }
    }
}
