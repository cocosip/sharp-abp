using AutoMapper;

namespace SharpAbp.MinId
{
    public class MinIdApplicationAutoMapperProfile : Profile
    {
        public MinIdApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            CreateMap<MinIdInfo, MinIdInfoDto>();
            CreateMap<MinIdInfo, MinIdInfoCacheItem>();
            CreateMap<MinIdToken, MinIdTokenDto>();
            CreateMap<MinIdToken, MinIdTokenCacheItem>();
            CreateMap<SegmentId, SegmentIdDto>();
        }
    }
}