using System;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public enum OperationRateLimitingPartitionKind
    {
        Global,
        Parameter,
        Argument,
        CurrentUser,
        CurrentTenant,
        Email,
        PhoneNumber,
        Resolver
    }

    public sealed class OperationRateLimitingPartition
    {
        public string Name { get; }

        public OperationRateLimitingPartitionKind Kind { get; }

        public string? ArgumentName { get; }

        public string? ResolverName { get; }

        private OperationRateLimitingPartition(string name, OperationRateLimitingPartitionKind kind,
            string? argumentName = null, string? resolverName = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new OperationRateLimitingConfigurationException("Partition name must be nonempty.");
            }

            if (kind == OperationRateLimitingPartitionKind.Argument && string.IsNullOrWhiteSpace(argumentName))
            {
                throw new OperationRateLimitingConfigurationException("Argument partition name must be nonempty.");
            }

            if (kind == OperationRateLimitingPartitionKind.Resolver && string.IsNullOrWhiteSpace(resolverName))
            {
                throw new OperationRateLimitingConfigurationException("Resolver name must be nonempty.");
            }

            Name = name;
            Kind = kind;
            ArgumentName = argumentName;
            ResolverName = resolverName;
        }

        public static OperationRateLimitingPartition Global()
        {
            return new OperationRateLimitingPartition("Global", OperationRateLimitingPartitionKind.Global);
        }

        public static OperationRateLimitingPartition Parameter()
        {
            return new OperationRateLimitingPartition("Parameter", OperationRateLimitingPartitionKind.Parameter);
        }

        public static OperationRateLimitingPartition Argument(string argumentName)
        {
            return new OperationRateLimitingPartition(argumentName, OperationRateLimitingPartitionKind.Argument,
                argumentName: argumentName);
        }

        public static OperationRateLimitingPartition CurrentUser()
        {
            return new OperationRateLimitingPartition("CurrentUser", OperationRateLimitingPartitionKind.CurrentUser);
        }

        public static OperationRateLimitingPartition CurrentTenant()
        {
            return new OperationRateLimitingPartition("CurrentTenant", OperationRateLimitingPartitionKind.CurrentTenant);
        }

        public static OperationRateLimitingPartition Email(string? argumentName = null)
        {
            return new OperationRateLimitingPartition("Email", OperationRateLimitingPartitionKind.Email,
                argumentName: argumentName);
        }

        public static OperationRateLimitingPartition PhoneNumber(string? argumentName = null)
        {
            return new OperationRateLimitingPartition("PhoneNumber", OperationRateLimitingPartitionKind.PhoneNumber,
                argumentName: argumentName);
        }

        public static OperationRateLimitingPartition Resolver(string resolverName)
        {
            return new OperationRateLimitingPartition(resolverName, OperationRateLimitingPartitionKind.Resolver,
                resolverName: resolverName);
        }

        public static OperationRateLimitingPartition Custom(string name, string resolverName)
        {
            return new OperationRateLimitingPartition(name, OperationRateLimitingPartitionKind.Resolver,
                resolverName: resolverName);
        }

        internal static OperationRateLimitingPartition FromLegacyName(string name)
        {
            switch (name)
            {
                case "Global":
                    return Global();
                case "Parameter":
                    return Parameter();
                case "CurrentUser":
                    return CurrentUser();
                case "CurrentTenant":
                    return CurrentTenant();
                case "Email":
                    return Email();
                case "PhoneNumber":
                    return PhoneNumber();
                default:
                    return Resolver(name);
            }
        }

        internal string GetIdentity()
        {
            return Kind + ":" + Name + ":" + (ArgumentName ?? string.Empty) + ":" + (ResolverName ?? string.Empty);
        }
    }
}
