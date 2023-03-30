using SharpAbp.Abp.TenancyGrouping;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupMapper : IObjectMapper<TenantGroup, TenantGroupConfiguration>, ITransientDependency
    {
        public TenantGroupConfiguration Map(TenantGroup source)
        {
            if (source != null)
            {
                var configuration = new TenantGroupConfiguration()
                {
                    Id = source.Id,
                    Name = source.Name,
                    IsActive = source.IsActive,
                    ConnectionStrings = new ConnectionStrings(),
                    Tenants = new List<Guid>()
                };
                if (source.ConnectionStrings != null)
                {
                    foreach (var connectionString in source.ConnectionStrings)
                    {
                        configuration.ConnectionStrings[connectionString.Name] = connectionString.Value;
                    }
                }

                if (source.Tenants != null)
                {
                    foreach (var tenant in source.Tenants)
                    {
                        configuration.Tenants.Add(tenant.TenantId);
                    }
                }


            }
            return null;
        }

        public TenantGroupConfiguration Map(TenantGroup source, TenantGroupConfiguration destination)
        {
            if (destination != null && source != null)
            {

            }

            return destination;
        }
    }
}
