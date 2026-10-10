using System.Collections.Generic;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingContext
    {
        public string? Parameter { get; set; }

        public OperationRateLimitingArguments Arguments { get; }

        public string? CurrentPartitionName { get; internal set; }

        public object? CurrentPartitionValue { get; internal set; }

        public IDictionary<string, object?> ExtraProperties { get; } = new Dictionary<string, object?>();

        public OperationRateLimitingContext()
        {
            Arguments = new OperationRateLimitingArguments();
        }

        public OperationRateLimitingContext(OperationRateLimitingArguments arguments)
        {
            Arguments = arguments ?? throw new System.ArgumentNullException(nameof(arguments));
        }

        public virtual object? GetArgument(string name)
        {
            return Arguments.GetValue(name);
        }

        public virtual T GetArgument<T>(string name)
        {
            return Arguments.GetValue<T>(name);
        }

        internal OperationRateLimitingContext CreatePartitionContext(
            OperationRateLimitingPartition partition, object? partitionValue)
        {
            var context = new OperationRateLimitingContext(Arguments)
            {
                Parameter = partitionValue as string ?? Parameter,
                CurrentPartitionName = partition.Name,
                CurrentPartitionValue = partitionValue
            };

            foreach (var property in ExtraProperties)
            {
                context.ExtraProperties[property.Key] = property.Value;
            }

            return context;
        }
    }
}
