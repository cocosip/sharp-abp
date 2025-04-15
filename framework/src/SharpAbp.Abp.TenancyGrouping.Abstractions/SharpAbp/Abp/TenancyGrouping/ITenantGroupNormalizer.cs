namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupNormalizer
    {
        string? NormalizeName(string? name);
    }
}
