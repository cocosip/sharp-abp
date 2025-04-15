namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ICurrentTenantGroupAccessor
    {
        BasicTenantGroupInfo? Current { get; set; }
    }
}
