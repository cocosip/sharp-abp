﻿using System;
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
    /// <summary>
    /// Represents a group of tenants.
    /// </summary>
    public class TenantGroup : AuditedAggregateRoot<Guid>, IHasEntityVersion
    {
        /// <summary>
        /// Gets or sets the name of the tenant group.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name of the tenant group.
        /// </summary>
        public virtual string NormalizedName { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant group is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the entity version.
        /// </summary>
        public int EntityVersion { get; protected set; }

        /// <summary>
        /// Gets or sets the connection strings associated with the tenant group.
        /// </summary>
        public virtual ICollection<TenantGroupConnectionString> ConnectionStrings { get; protected set; } = [];

        /// <summary>
        /// Gets or sets the tenants associated with the tenant group.
        /// </summary>
        public virtual ICollection<TenantGroupTenant> Tenants { get; protected set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroup"/> class.
        /// </summary>
        public TenantGroup()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroup"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the tenant group.</param>
        /// <param name="name">The name of the tenant group.</param>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        /// <param name="isActive">A value indicating whether the tenant group is active.</param>
        public TenantGroup(Guid id, [NotNull] string name, [CanBeNull] string normalizedName, bool isActive) : base(id)
        {
            SetName(name);
            SetNormalizedName(normalizedName);
            IsActive = isActive;
        }

        /// <summary>
        /// Adds a tenant to the tenant group.
        /// </summary>
        /// <param name="tenant">The tenant to add.</param>
        /// <exception cref="AbpException">Thrown when a tenant with the same ID already exists in the group.</exception>
        public virtual void AddTenant(TenantGroupTenant tenant)
        {
            if (Tenants.Any(x => x.TenantId == tenant.Id))
            {
                throw new AbpException($"TenantGroup duplicate tenantId: {tenant.TenantId}");
            }
            Tenants.Add(tenant);
        }

        /// <summary>
        /// Removes a tenant from the tenant group by its ID.
        /// </summary>
        /// <param name="tenantGroupTenantId">The ID of the tenant to remove.</param>
        public virtual void RemoveTenant(Guid tenantGroupTenantId)
        {
            var tenant = Tenants.FirstOrDefault(x => x.Id == tenantGroupTenantId);
            if (tenant != null)
            {
                Tenants.Remove(tenant);
            }
        }

        /// <summary>
        /// Removes a tenant from the tenant group by its tenant ID.
        /// </summary>
        /// <param name="tenantId">The tenant ID of the tenant to remove.</param>
        public virtual void RemoveTenantByTenantId(Guid tenantId)
        {
            var tenant = Tenants.FirstOrDefault(x => x.TenantId == tenantId);
            if (tenant != null)
            {
                Tenants.Remove(tenant);
            }
        }

        /// <summary>
        /// Gets a connection string by its name.
        /// </summary>
        /// <param name="name">The name of the connection string.</param>
        /// <returns>The connection string with the specified name, or null if not found.</returns>
        public virtual TenantGroupConnectionString GetConnectionString(string name)
        {
            return ConnectionStrings.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Gets the default connection string.
        /// </summary>
        /// <returns>The default connection string, or null if not found.</returns>
        public virtual TenantGroupConnectionString GetDefaultConnectionString()
        {
            return GetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }


        /// <summary>
        /// Sets a connection string with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the connection string.</param>
        /// <param name="connectionString">The value of the connection string.</param>
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

        /// <summary>
        /// Finds the default connection string.
        /// </summary>
        /// <returns>The default connection string, or null if not found.</returns>
        [CanBeNull]
        public virtual string FindDefaultConnectionString()
        {
            return FindConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }

        /// <summary>
        /// Finds a connection string by its name.
        /// </summary>
        /// <param name="name">The name of the connection string.</param>
        /// <returns>The value of the connection string with the specified name, or null if not found.</returns>
        [CanBeNull]
        public virtual string FindConnectionString(string name)
        {
            return ConnectionStrings.FirstOrDefault(c => c.Name == name)?.Value;
        }

        /// <summary>
        /// Sets the default connection string.
        /// </summary>
        /// <param name="connectionString">The value of the default connection string.</param>
        public virtual void SetDefaultConnectionString(string connectionString)
        {
            SetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName, connectionString);
        }

        /// <summary>
        /// Removes the default connection string.
        /// </summary>
        public virtual void RemoveDefaultConnectionString()
        {
            RemoveConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
        }

        /// <summary>
        /// Removes a connection string by its name.
        /// </summary>
        /// <param name="name">The name of the connection string to remove.</param>
        public virtual void RemoveConnectionString(string name)
        {
            var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

            if (tenantConnectionString != null)
            {
                ConnectionStrings.Remove(tenantConnectionString);
            }
        }

        /// <summary>
        /// Sets the name of the tenant group.
        /// </summary>
        /// <param name="name">The name of the tenant group.</param>
        protected internal virtual void SetName([NotNull] string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
        }

        /// <summary>
        /// Sets the normalized name of the tenant group.
        /// </summary>
        /// <param name="normalizedName">The normalized name of the tenant group.</param>
        protected internal virtual void SetNormalizedName([CanBeNull] string normalizedName)
        {
            NormalizedName = normalizedName;
        }

        /// <summary>
        /// Sets whether the tenant group is active.
        /// </summary>
        /// <param name="isActive">A value indicating whether the tenant group is active.</param>
        protected internal virtual void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}