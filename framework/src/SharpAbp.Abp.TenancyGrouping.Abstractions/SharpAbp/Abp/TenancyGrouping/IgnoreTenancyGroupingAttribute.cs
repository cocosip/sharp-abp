using System;

namespace SharpAbp.Abp.TenancyGrouping
{
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreTenancyGroupingAttribute : Attribute
    {
    }
}
