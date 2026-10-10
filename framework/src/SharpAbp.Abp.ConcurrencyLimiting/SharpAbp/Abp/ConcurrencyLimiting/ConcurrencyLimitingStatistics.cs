
namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>A point-in-time snapshot that does not reserve a permit.</summary>
    public sealed class ConcurrencyLimitingStatistics
    {
        public bool IsEnabled { get; }
        public long AvailablePermits { get; }
        public long QueuedOperations { get; }
        internal ConcurrencyLimitingStatistics(bool isEnabled, long availablePermits, long queuedOperations)
        {
            IsEnabled = isEnabled;
            AvailablePermits = availablePermits;
            QueuedOperations = queuedOperations;
        }
    }

}
