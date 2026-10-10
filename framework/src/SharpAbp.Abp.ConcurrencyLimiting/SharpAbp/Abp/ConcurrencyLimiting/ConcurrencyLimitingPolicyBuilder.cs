using System;
using System.Threading.RateLimiting;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public sealed class ConcurrencyLimitingPolicyBuilder
    {
        internal int PermitLimit { get; private set; }
        internal int QueueLimit { get; private set; }
        internal TimeSpan QueueTimeout { get; private set; } = TimeSpan.FromSeconds(30);
        internal QueueProcessingOrder QueueProcessingOrder { get; private set; } = QueueProcessingOrder.OldestFirst;
        internal ConcurrencyLimitingPartitionKind PartitionKind { get; private set; }
        internal string? ResolverName { get; private set; }
        internal bool IsMultiTenant { get; private set; }
        internal string ErrorCode { get; private set; } = ConcurrencyLimitingErrorCodes.LimitExceeded;
        public ConcurrencyLimitingPolicyBuilder()
        {
        }

        internal ConcurrencyLimitingPolicyBuilder(ConcurrencyLimitingPolicy policy)
        {
            PermitLimit = policy.PermitLimit;
            QueueLimit = policy.QueueLimit;
            QueueTimeout = policy.QueueTimeout;
            QueueProcessingOrder = policy.QueueProcessingOrder;
            PartitionKind = policy.PartitionKind;
            ResolverName = policy.ResolverName;
            IsMultiTenant = policy.IsMultiTenant;
            ErrorCode = policy.ErrorCode;
        }

        public ConcurrencyLimitingPolicyBuilder WithConcurrency(int permitLimit, int queueLimit = 0)
        {
            PermitLimit = permitLimit;
            QueueLimit = queueLimit;
            return this;
        }

        public ConcurrencyLimitingPolicyBuilder WithQueueTimeout(TimeSpan timeout)
        {
            QueueTimeout = timeout;
            return this;
        }

        public ConcurrencyLimitingPolicyBuilder WithQueueProcessingOrder(QueueProcessingOrder order)
        {
            QueueProcessingOrder = order;
            return this;
        }

        public ConcurrencyLimitingPolicyBuilder WithErrorCode(string code)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(code, nameof(code));
            ErrorCode = code;
            return this;
        }

        public ConcurrencyLimitingPolicyBuilder WithMultiTenancy(bool enabled = true)
        {
            IsMultiTenant = enabled;
            return this;
        }

        public ConcurrencyLimitingPolicyBuilder PartitionGlobally()
        {
            return SetPartition(ConcurrencyLimitingPartitionKind.Global);
        }

        public ConcurrencyLimitingPolicyBuilder PartitionByParameter()
        {
            return SetPartition(ConcurrencyLimitingPartitionKind.Parameter);
        }

        public ConcurrencyLimitingPolicyBuilder PartitionByCurrentUser()
        {
            return SetPartition(ConcurrencyLimitingPartitionKind.CurrentUser);
        }

        public ConcurrencyLimitingPolicyBuilder PartitionByCurrentTenant()
        {
            return SetPartition(ConcurrencyLimitingPartitionKind.CurrentTenant);
        }

        public ConcurrencyLimitingPolicyBuilder PartitionBy(string resolverName)
        {
            SharpAbpConcurrencyLimitingOptions.ValidateName(resolverName, nameof(resolverName));
            SetPartition(ConcurrencyLimitingPartitionKind.Custom);
            ResolverName = resolverName;
            return this;
        }

        private ConcurrencyLimitingPolicyBuilder SetPartition(ConcurrencyLimitingPartitionKind kind)
        {
            PartitionKind = kind;
            ResolverName = null;
            return this;
        }

        internal ConcurrencyLimitingPolicy Build(string name)
        {
            if (PermitLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(PermitLimit));
            }
            if (QueueLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(QueueLimit));
            }
            // CancellationTokenSource supports a finite, bounded timer on every production TFM.
            if (QueueTimeout <= TimeSpan.Zero || QueueTimeout.TotalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(QueueTimeout));
            }
            if (QueueProcessingOrder != QueueProcessingOrder.OldestFirst && QueueProcessingOrder != QueueProcessingOrder.NewestFirst)
            {
                throw new ArgumentOutOfRangeException(nameof(QueueProcessingOrder));
            }
            return new ConcurrencyLimitingPolicy(name, this);
        }
    }
}
