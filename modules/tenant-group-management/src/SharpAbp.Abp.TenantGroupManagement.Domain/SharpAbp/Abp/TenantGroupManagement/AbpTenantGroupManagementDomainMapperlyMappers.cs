using Riok.Mapperly.Abstractions;
using SharpAbp.Abp.TenancyGrouping;
using Volo.Abp.Mapperly;

namespace SharpAbp.Abp.TenantGroupManagement
{

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantGroupToTenantGroupConfigurationMapper : MapperBase<TenantGroup, TenantGroupConfiguration>
    {
        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.Tenants))]
        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.ConnectionStrings))]
        public override partial TenantGroupConfiguration Map(TenantGroup source);

        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.Tenants))]
        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.ConnectionStrings))]
        public override partial void Map(TenantGroup source, TenantGroupConfiguration destination);

        public override void AfterMap(TenantGroup source, TenantGroupConfiguration destination)
        {
            destination.ConnectionStrings = [];
            foreach (var connectionString in source.ConnectionStrings)
            {
                destination.ConnectionStrings[connectionString.Name] = connectionString.Value;
            }

            destination.Tenants = [];
            foreach (var tenant in source.Tenants)
            {
                destination.Tenants.Add(tenant.TenantId);
            }

        }
    }


    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class TenantGroupToTenantGroupEtoMapper : MapperBase<TenantGroup, TenantGroupEto>
    {

        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.Tenants))]
        public override partial TenantGroupEto Map(TenantGroup source);

        [MapperIgnoreTarget(nameof(TenantGroupConfiguration.Tenants))]
        public override partial void Map(TenantGroup source, TenantGroupEto destination);

        public override void AfterMap(TenantGroup source, TenantGroupEto destination)
        {
            destination.Tenants = [];
            foreach (var tenant in source.Tenants)
            {
                destination.Tenants.Add(tenant.TenantId);
            }

        }
    }

    // public class AbpTenantGroupManagementDomainMappingProfile : Profile
    // {
    //     public AbpTenantGroupManagementDomainMappingProfile()
    //     {
    //         CreateMap<TenantGroup, TenantGroupConfiguration>()
    //             .ForMember(ti => ti.ConnectionStrings, opts =>
    //             {
    //                 opts.MapFrom((tenantGroup, ti) =>
    //                 {
    //                     var connStrings = new ConnectionStrings();

    //                     foreach (var connectionString in tenantGroup.ConnectionStrings)
    //                     {
    //                         connStrings[connectionString.Name] = connectionString.Value;
    //                     }

    //                     return connStrings;
    //                 });
    //             })
    //             .ForMember(ti => ti.Tenants, opts =>
    //             {
    //                 opts.MapFrom((tenantGroup, ti) =>
    //                 {
    //                     var tenants = new List<Guid>();

    //                     foreach (var tenant in tenantGroup.Tenants)
    //                     {
    //                         tenants.Add(tenant.TenantId);
    //                     }
    //                     return tenants;
    //                 });
    //             });

    //         CreateMap<TenantGroup, TenantGroupEto>()
    //             .ForMember(ti => ti.Tenants, opts =>
    //             {
    //                 opts.MapFrom((tenantGroup, ti) =>
    //                 {
    //                     var tenants = new List<Guid>();

    //                     foreach (var tenant in tenantGroup.Tenants)
    //                     {
    //                         tenants.Add(tenant.TenantId);
    //                     }
    //                     return tenants;
    //                 });
    //             });
    //     }
    //}
}
