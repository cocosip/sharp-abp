
namespace SharpAbp.Abp.ConcurrencyLimiting
{
    public interface IConcurrencyLimitingPolicyProvider
    {
        ConcurrencyLimitingPolicy Get(string name);
    }

}
