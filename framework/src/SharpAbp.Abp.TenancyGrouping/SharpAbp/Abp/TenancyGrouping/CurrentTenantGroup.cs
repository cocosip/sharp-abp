using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TenancyGrouping
{
    public class CurrentTenantGroup : ICurrentTenantGroup, ITransientDependency
    {
        public virtual bool IsAvailable => Id.HasValue;

        public virtual Guid? Id => _currentTenantGroupAccessor.Current?.GroupId;

        public string? Name => _currentTenantGroupAccessor.Current?.Name;

        public List<Guid>? Tenants => _currentTenantGroupAccessor.Current?.Tenants;

        private readonly ICurrentTenantGroupAccessor _currentTenantGroupAccessor;

        public CurrentTenantGroup(ICurrentTenantGroupAccessor currentTenantGroupAccessor)
        {
            _currentTenantGroupAccessor = currentTenantGroupAccessor;
        }

        public IDisposable Change(Guid? id, string? name = null, List<Guid>? tenants = null)
        {
            return SetCurrent(id, name);
        }

        private IDisposable SetCurrent(Guid? groupId, string? name = null, List<Guid>? tenants = null)
        {
            var parentScope = _currentTenantGroupAccessor.Current;
            _currentTenantGroupAccessor.Current = new BasicTenantGroupInfo(groupId, name,tenants);

            return new DisposeAction<ValueTuple<ICurrentTenantGroupAccessor, BasicTenantGroupInfo?>>(static (state) =>
            {
                var (currentTenantAccessor, parentScope) = state;
                currentTenantAccessor.Current = parentScope;
            }, (_currentTenantGroupAccessor, parentScope));
        }
    }
}
