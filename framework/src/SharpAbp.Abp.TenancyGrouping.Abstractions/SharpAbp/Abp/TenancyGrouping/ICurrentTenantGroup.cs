using System;
using System.Collections.Generic;

namespace SharpAbp.Abp.TenancyGrouping
{
    public interface ICurrentTenantGroup
    {
        bool IsAvailable { get; }
        Guid? Id { get; }
        string? Name { get; }
        List<Guid>? Tenants { get; }
        IDisposable Change(Guid? id, string? name = null, List<Guid>? tenants = null);
    }
}
