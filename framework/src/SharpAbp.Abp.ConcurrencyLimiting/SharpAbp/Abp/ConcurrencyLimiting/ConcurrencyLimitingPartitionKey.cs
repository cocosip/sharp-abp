using System;

namespace SharpAbp.Abp.ConcurrencyLimiting
{
    // Structural fields prevent delimiter collisions; no case or whitespace normalization.
    public readonly struct ConcurrencyLimitingPartitionKey : IEquatable<ConcurrencyLimitingPartitionKey>
    {
        public string Value { get; }
        public Guid? TenantId { get; }
        public ConcurrencyLimitingPartitionKey(string value, Guid? tenantId)
        {
            Value = value;
            TenantId = tenantId;
        }

        public bool Equals(ConcurrencyLimitingPartitionKey other)
        {
            return StringComparer.Ordinal.Equals(Value, other.Value) && TenantId == other.TenantId;
        }

        public override bool Equals(object? obj)
        {
            return obj is ConcurrencyLimitingPartitionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return unchecked(StringComparer.Ordinal.GetHashCode(Value) * 397 ^ TenantId.GetHashCode());
        }
    }

}
