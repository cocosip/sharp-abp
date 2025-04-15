namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ITenantGroupResolveResultAccessor
    {
        TenantGroupResolveResult? Result { get; set; }
    }
}
