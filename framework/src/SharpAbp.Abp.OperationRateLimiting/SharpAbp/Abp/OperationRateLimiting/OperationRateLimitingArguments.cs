using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public sealed class OperationRateLimitingArguments
    {
        private readonly IDictionary<string, object?> values =
            new Dictionary<string, object?>(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, object?> Values =>
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>(values);

        public OperationRateLimitingArguments Add(string name, object? value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new OperationRateLimitingContextException("Operation rate limiting argument name must be nonempty.");
            }

            if (values.ContainsKey(name))
            {
                throw new OperationRateLimitingContextException("Operation rate limiting argument already exists: " + name);
            }

            values.Add(name, value);
            return this;
        }

        public bool TryGetValue(string name, out object? value)
        {
            return values.TryGetValue(name, out value);
        }

        public object? GetValue(string name)
        {
            if (!values.TryGetValue(name, out var value) || value == null)
            {
                throw new OperationRateLimitingContextException("Missing operation rate limiting argument: " + name + ".");
            }

            return value;
        }

        public T GetValue<T>(string name)
        {
            var value = GetValue(name);
            if (value is T typedValue)
            {
                return typedValue;
            }

            throw new OperationRateLimitingContextException(
                "Operation rate limiting argument has an invalid type: " + name + ".");
        }

        public static OperationRateLimitingArguments Create(params (string Name, object? Value)[] arguments)
        {
            var result = new OperationRateLimitingArguments();
            foreach (var argument in arguments)
            {
                result.Add(argument.Name, argument.Value);
            }

            return result;
        }
    }
}
