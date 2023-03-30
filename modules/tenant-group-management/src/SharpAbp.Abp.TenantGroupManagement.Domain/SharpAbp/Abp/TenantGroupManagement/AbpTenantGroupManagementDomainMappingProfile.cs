using AutoMapper;
using SharpAbp.Abp.TenancyGrouping;
using System;
using System.Collections.Generic;
using Volo.Abp.Data;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class AbpTenantGroupManagementDomainMappingProfile : Profile
    {
        public AbpTenantGroupManagementDomainMappingProfile()
        {
            CreateMap<TenantGroup, TenantGroupConfiguration>()
                .ForMember(ti => ti.ConnectionStrings, opts =>
                {
                    opts.MapFrom((tenantGroup, ti) =>
                    {
                        var connStrings = new ConnectionStrings();

                        foreach (var connectionString in tenantGroup.ConnectionStrings)
                        {
                            connStrings[connectionString.Name] = connectionString.Value;
                        }

                        return connStrings;
                    });
                })
                .ForMember(ti => ti.Tenants, opts =>
                {
                    opts.MapFrom((tenantGroup, ti) =>
                    {
                        var tenants = new List<Guid>();

                        foreach (var tenant in tenantGroup.Tenants)
                        {
                            tenants.Add(tenant.TenantId);
                        }
                        return tenants;
                    });
                });
        }
    }
}
