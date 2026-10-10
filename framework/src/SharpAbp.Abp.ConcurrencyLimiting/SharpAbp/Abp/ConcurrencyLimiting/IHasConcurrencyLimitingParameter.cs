
namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public interface IHasConcurrencyLimitingParameter
    {
        string? GetPartitionParameter();
    }

}
