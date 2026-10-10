using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Volo.Abp.DynamicProxy;


namespace SharpAbp.Abp.ConcurrencyLimiting
{
    internal static class ConcurrencyLimitingInvocationContextResolver
    {
        private static readonly ConditionalWeakTable<Type, ConcurrentDictionary<MethodInfo, InvocationMetadata>> MetadataCache =
            new ConditionalWeakTable<Type, ConcurrentDictionary<MethodInfo, InvocationMetadata>>();

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

        public static bool HasPolicy(Type implementationType)
        {
            if (implementationType.IsDefined(typeof(ConcurrencyLimitingAttribute), true) ||
                implementationType.GetMethods().Any(method =>
                    method.IsDefined(typeof(ConcurrencyLimitingAttribute), true)))
            {
                return true;
            }

            foreach (var interfaceType in implementationType.GetInterfaces())
            {
                if (interfaceType.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
                {
                    continue;
                }
                if (interfaceType.IsDefined(typeof(ConcurrencyLimitingAttribute), true) ||
                    interfaceType.GetMethods().Any(method =>
                        method.IsDefined(typeof(ConcurrencyLimitingAttribute), true)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Warm(Type implementationType)
        {
            var methods = MetadataCache.GetValue(implementationType,
                _ => new ConcurrentDictionary<MethodInfo, InvocationMetadata>());
            var hasPolicy = false;
            var classHasPolicy = implementationType.IsDefined(typeof(ConcurrencyLimitingAttribute), true);
            foreach (var method in implementationType.GetMethods())
            {
                if (method.DeclaringType == typeof(object) ||
                    (!classHasPolicy && !method.IsDefined(typeof(ConcurrencyLimitingAttribute), true) &&
                     !HasInterfacePolicy(implementationType, method)))
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
            ConcurrentDictionary<MethodInfo, InvocationMetadata> methods;
            while (true)
            {
                if (MetadataCache.TryGetValue(type, out methods))
                {
                    break;
                }
                if (type.BaseType == null)
                {
                    methods = MetadataCache.GetValue(type,
                        _ => new ConcurrentDictionary<MethodInfo, InvocationMetadata>());
                    break;
                }
                type = type.BaseType;
            }
            if (actualType != type)
            {
                MetadataCache.GetValue(actualType, _ => methods);
            }
            return methods.GetOrAdd(method, candidate => BuildMetadata(type, candidate));
        }

        internal static int GetCachedMethodCount(Type implementationType)
        {
            return MetadataCache.TryGetValue(implementationType, out var methods) ? methods.Count : 0;
        }

        internal static object? GetCachedMetadata(Type implementationType, MethodInfo method)
        {
            return MetadataCache.TryGetValue(implementationType, out var methods) &&
                   methods.TryGetValue(method, out var metadata) ? metadata : null;
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
                policy = GetInterfacePolicy(type, method);
            }
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

        private static bool HasInterfacePolicy(Type implementationType, MethodInfo method)
        {
            return GetInterfacePolicy(implementationType, method) != null;
        }

        private static string? GetInterfacePolicy(Type implementationType, MethodInfo method)
        {
            foreach (var interfaceType in implementationType.GetInterfaces())
            {
                if (interfaceType.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
                {
                    continue;
                }

                InterfaceMapping map;
                try
                {
                    map = implementationType.GetInterfaceMap(interfaceType);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                for (var index = 0; index < map.TargetMethods.Length; index++)
                {
                    if (map.TargetMethods[index] != method)
                    {
                        continue;
                    }

                    var interfaceMethod = map.InterfaceMethods[index];
                    if (interfaceMethod.IsDefined(typeof(DisableConcurrencyLimitingAttribute), true))
                    {
                        return null;
                    }

                    return interfaceMethod.GetCustomAttribute<ConcurrencyLimitingAttribute>(true)?.PolicyName ??
                        interfaceType.GetCustomAttribute<ConcurrencyLimitingAttribute>(true)?.PolicyName;
                }
            }

            return null;
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
