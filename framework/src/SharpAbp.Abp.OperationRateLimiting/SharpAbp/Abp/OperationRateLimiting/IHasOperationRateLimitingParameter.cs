

namespace SharpAbp.Abp.OperationRateLimiting
{
    public interface IHasOperationRateLimitingParameter
    {
        string? GetPartitionParameter();
    }
}
