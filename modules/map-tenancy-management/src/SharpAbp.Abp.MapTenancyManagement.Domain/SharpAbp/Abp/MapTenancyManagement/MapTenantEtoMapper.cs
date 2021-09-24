using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using System;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenantEtoMapper : IObjectMapper<MapTenant, MapTenantEto>, ITransientDependency
    {
        public MapTenantEto Map(MapTenant source)
        {
            if (source != null)
            {
                return new MapTenantEto()
                {
                    Id = source.Id,
                    TenantId = source.TenantId,
                    Code = source.Code,
                    MapCode = source.MapCode
                };
            }

            return null;
        }

        public MapTenantEto Map(MapTenant source, MapTenantEto destination)
        {
            if (destination == null)
            {
                destination = new MapTenantEto();
            }
            if (source != null)
            {
                destination.Id = source.Id;
                destination.TenantId = source.TenantId;
                if (!source.Code.IsNullOrWhiteSpace())
                {
                    destination.Code = source.Code;
                }

                if (!source.MapCode.IsNullOrWhiteSpace())
                {
                    destination.MapCode = source.MapCode;
                }

            }
            return destination;
        }
    }
}