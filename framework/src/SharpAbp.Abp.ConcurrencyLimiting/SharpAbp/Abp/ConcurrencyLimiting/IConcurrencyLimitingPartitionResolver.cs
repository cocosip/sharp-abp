
namespace SharpAbp.Abp.ConcurrencyLimiting
{
    /// <summary>Resolves a partition in the current service scope.</summary>
    public interface IConcurrencyLimitingPartitionResolver
    {
        ConcurrencyLimitingPartitionKey Resolve(ConcurrencyLimitingPolicy policy, ConcurrencyLimitingContext context);
    }

}
