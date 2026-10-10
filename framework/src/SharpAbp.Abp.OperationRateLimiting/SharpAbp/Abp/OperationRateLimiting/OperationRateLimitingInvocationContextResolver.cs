using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DynamicProxy;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public static class OperationRateLimitingInvocationContextResolver
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<RuntimeMethodHandle, InvocationMetadata>> Metadata =
            new ConcurrentDictionary<Type, IReadOnlyDictionary<RuntimeMethodHandle, InvocationMetadata>>();
        private static readonly InvocationMetadata Unrestricted = new InvocationMetadata(null, -1, -1, null);

        public static bool Warm(Type implementationType)
        {
            if (Metadata.ContainsKey(implementationType))
            {
                return true;
            }
            if (implementationType.IsDefined(typeof(DisableOperationRateLimitingAttribute), true))
            {
                return false;
            }

            var classPolicy = implementationType.GetCustomAttribute<OperationRateLimitingAttribute>(true)?.PolicyName;
            var methods = new Dictionary<RuntimeMethodHandle, InvocationMetadata>();
            foreach (var method in implementationType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (method.DeclaringType == typeof(object) || method.IsDefined(typeof(DisableOperationRateLimitingAttribute), true))
                {
                    continue;
                }
                var policy = method.GetCustomAttribute<OperationRateLimitingAttribute>(true)?.PolicyName ?? classPolicy;
                if (policy != null)
                {
                    methods.Add(method.MethodHandle, CreateMetadata(method, policy));
                }
            }
            if (methods.Count == 0)
            {
                return false;
            }
            Metadata.TryAdd(implementationType, methods);
            return true;
        }

        public static InvocationMetadata GetMetadata(Type implementationType, MethodInfo method)
        {
            if (!Metadata.TryGetValue(implementationType, out var methods))
            {
                // ABP class proxies may supply their generated type. Reuse the registered base metadata.
                var baseType = implementationType.BaseType;
                if (baseType == null || !Metadata.TryGetValue(baseType, out methods))
                {
                    return Unrestricted;
                }
                Metadata.TryAdd(implementationType, methods);
            }
            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                method = method.GetGenericMethodDefinition();
            }
            return methods.TryGetValue(method.MethodHandle, out var metadata) ? metadata : Unrestricted;
        }

        private static InvocationMetadata CreateMetadata(MethodInfo method, string policy)
        {
            var parameterIndex = -1;
            var tokenIndex = -1;
            string? error = null;
            var parameters = method.GetParameters();
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.IsDefined(typeof(RateLimitingParameterAttribute), true))
                {
                    if (parameterIndex >= 0)
                    {
                        error = "Multiple operation parameter markers are ambiguous.";
                    }
                    else if (parameter.ParameterType != typeof(string))
                    {
                        error = "RateLimitingParameter must mark a string parameter.";
                    }
                    parameterIndex = index;
                }
                if (parameter.ParameterType == typeof(CancellationToken))
                {
                    if (tokenIndex >= 0)
                    {
                        error = "Multiple CancellationToken arguments are ambiguous.";
                    }
                    tokenIndex = index;
                }
            }
            if (method.ReturnType != typeof(Task) &&
                !(method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
            {
                error = "OperationRateLimiting supports only Task and Task<T> methods.";
            }
            return new InvocationMetadata(policy, parameterIndex, tokenIndex, error);
        }

        public static OperationRateLimitingContext ResolveContext(IAbpMethodInvocation invocation, InvocationMetadata metadata)
        {
            if (metadata.ParameterIndex >= 0)
            {
                return new OperationRateLimitingContext { Parameter = (string?)invocation.Arguments[metadata.ParameterIndex] };
            }
            IHasOperationRateLimitingParameter? candidate = null;
            foreach (var argument in invocation.Arguments)
            {
                if (argument is IHasOperationRateLimitingParameter dto)
                {
                    if (candidate != null)
                    {
                        throw new OperationRateLimitingConfigurationException("Multiple operation parameter DTOs are ambiguous.");
                    }
                    candidate = dto;
                }
            }
            return new OperationRateLimitingContext { Parameter = candidate?.GetPartitionParameter() };
        }

        public class InvocationMetadata
        {
            public string? PolicyName { get; }
            public int ParameterIndex { get; }
            public int TokenIndex { get; }
            public string? Error { get; }

            public InvocationMetadata(string? policyName, int parameterIndex, int tokenIndex, string? error)
            {
                PolicyName = policyName;
                ParameterIndex = parameterIndex;
                TokenIndex = tokenIndex;
                Error = error;
            }
        }
    }
}
