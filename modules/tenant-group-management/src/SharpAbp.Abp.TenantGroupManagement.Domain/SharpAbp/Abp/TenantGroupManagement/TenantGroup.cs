using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.TenantManagement;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroup : AuditedAggregateRoot<Guid>, IHasEntityVersion
    {
        public virtual string Name { get; set; }

        public virtual string NormalizedName { get; protected set; }

        public virtual bool IsActive { get; set; }

        public int EntityVersion { get; protected set; }

        public virtual ICollection<TenantGroupConnectionString> ConnectionStrings { get; protected set; }

        public virtual ICollection<TenantGroupTenant> Tenants { get; protected set; }

        public TenantGroup()
        {
            ConnectionStrings = [];
            Tenants = [];
        }

        public TenantGroup(Guid id, [NotNull] string name, [CanBeNull] string normalizedName, bool isActive) : this()
        {
            Id = id;
            SetName(name);
            SetNormalizedName(normalizedName);
            IsActive = isActive;
        }

        public virtual void AddTenant(TenantGroupTenant tenant)
        {
            if (Tenants.Any(x => x.TenantId == tenant.Id))
            {
                throw new AbpException($"Dumplicate tenantId: {tenant.Id}");
            }
            Tenants.Add(tenant);
        }

        public virtual void RemoveTenant(Guid tenantGroupTenantId)
        {
            var tenant = Tenants.FirstOrDefault(x => x.Id == tenantGroupTenantId);
            if (tenant != null)
            {
                Tenants.Remove(tenant);
            }
        }

        public virtual void RemoveTenantByTenantId(Guid tenantId)
        {
            var tenant = Tenants.FirstOrDefault(x => x.TenantId == tenantId);
            if (tenant != null)
            {
                Tenants.Remove(tenant);
            }
        }

        public virtual TenantGroupConnectionString GetConnectionString(string name)
        {
            return ConnectionStrings.FirstOrDefault(x => x.Name == name);
        }

        public virtual TenantGroupConnectionString GetDefaultConnectionString()
        {
            return GetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }


        public virtual void SetConnectionString(string name, string connectionString)
        {
            var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

            if (tenantConnectionString != null)
            {
                tenantConnectionString.SetValue(connectionString);
            }
            else
            {
                ConnectionStrings.Add(new TenantGroupConnectionString(Id, name, connectionString));
            }
        }

        [CanBeNull]
        public virtual string FindDefaultConnectionString()
        {
            return FindConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }

        [CanBeNull]
        public virtual string FindConnectionString(string name)
        {
            return ConnectionStrings.FirstOrDefault(c => c.Name == name)?.Value;
        }


        public virtual void SetDefaultConnectionString(string connectionString)
        {
            SetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName, connectionString);
        }

        public virtual void RemoveDefaultConnectionString()
        {
            RemoveConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }

        public virtual void RemoveConnectionString(string name)
        {
            var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

            if (tenantConnectionString != null)
            {
                ConnectionStrings.Remove(tenantConnectionString);
            }
        }

        protected internal virtual void SetName([NotNull] string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
        }

        protected internal virtual void SetNormalizedName([CanBeNull] string normalizedName)
        {
            NormalizedName = normalizedName;
        }
        protected internal virtual void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
