using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.TenantManagement;

namespace SharpAbp.Abp.TenantGroupManagement
{
    /// <summary>
    /// Represents a connection string associated with a tenant group.
    /// This entity stores database connection strings or other connection configurations for tenant groups.
    /// </summary>
    public class TenantGroupConnectionString : Entity<Guid>
    {
        /// <summary>
        /// Gets or sets the identifier of the tenant group that owns this connection string.
        /// </summary>
        public virtual Guid TenantGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection string.
        /// This typically represents the purpose or type of the connection (e.g., "Default", "Logging", etc.).
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the actual connection string value.
        /// This contains the connection configuration details such as server, database, credentials, etc.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupConnectionString"/> class.
        /// </summary>
        public TenantGroupConnectionString()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantGroupConnectionString"/> class with the specified parameters.
        /// </summary>
        /// <param name="tenantGroupId">The identifier of the tenant group.</param>
        /// <param name="name">The name of the connection string.</param>
        /// <param name="value">The connection string value.</param>
        /// <exception cref="ArgumentException">Thrown when name or value is null, empty, or exceeds maximum length.</exception>
        public TenantGroupConnectionString(Guid tenantGroupId, [NotNull] string name, [NotNull] string value)
        {
            TenantGroupId = tenantGroupId;
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConnectionStringConsts.MaxNameLength);
            SetValue(value);
        }

        /// <summary>
        /// Sets the connection string value with validation.
        /// </summary>
        /// <param name="value">The connection string value to set.</param>
        /// <exception cref="ArgumentException">Thrown when value is null, empty, or exceeds maximum length.</exception>
        public virtual void SetValue([NotNull] string value)
        {
            Value = Check.NotNullOrWhiteSpace(value, nameof(value), TenantConnectionStringConsts.MaxValueLength);
        }

        /// <summary>
        /// Gets the composite key values for this entity.
        /// The composite key consists of TenantGroupId and Name.
        /// </summary>
        /// <returns>An array containing the key values: TenantGroupId and Name.</returns>
        public override object[] GetKeys()
        {
            return new object[] { TenantGroupId, Name };
        }
    }
}
