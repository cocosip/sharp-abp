using System;
using System.Threading.RateLimiting;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public enum ConcurrencyLimitingPartitionKind
    {
        Global, Parameter, CurrentUser, CurrentTenant, Custom
    }
    public sealed class ConcurrencyLimitingPolicy
    {
        public string Name { get; }
        public int PermitLimit { get; }
        public int QueueLimit { get; }
        public TimeSpan QueueTimeout { get; }
        public QueueProcessingOrder QueueProcessingOrder { get; }
        public ConcurrencyLimitingPartitionKind PartitionKind { get; }
        public string? ResolverName { get; }
        public bool IsMultiTenant { get; }
        public string ErrorCode { get; }
        internal ConcurrencyLimitingPolicy(string name, ConcurrencyLimitingPolicyBuilder builder)
        {
            Name = name;
            PermitLimit = builder.PermitLimit;
            QueueLimit = builder.QueueLimit;
            QueueTimeout = builder.QueueTimeout;
            QueueProcessingOrder = builder.QueueProcessingOrder;
            PartitionKind = builder.PartitionKind;
            ResolverName = builder.ResolverName;
            IsMultiTenant = builder.IsMultiTenant;
            ErrorCode = builder.ErrorCode;
        }
    }

}
